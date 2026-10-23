
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ObstaclePair : MonoBehaviour
{
    [Header("Movimentação")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Destruição")]
    [SerializeField] private float destroyX = -11f;

    [Header("Abertura Vertical")]
    [SerializeField] private Transform topPipe;
    [SerializeField] private Transform bottomPipe;
    [SerializeField] private float originalGap = 2.6f;

    [Header("Imagens dos Obstáculos")]
    [SerializeField] private Sprite[] obstacleSprites;

    [Header("Referências Visuais")]
    [SerializeField] private SpriteRenderer topVisual;
    [SerializeField] private SpriteRenderer bottomVisual;

    private Rigidbody2D rb;

    // O BoxCollider2D continua sendo usado como
    // referência para detectar a passagem do jogador.
    private Collider2D obstacleCollider;
    private Collider2D playerCollider;

    private GameManager gameManager;

    private Vector3 originalTopPosition;
    private Vector3 originalBottomPosition;

    private bool hasScored = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Recupera o BoxCollider2D usado na pontuação.
        obstacleCollider = GetComponentInChildren<BoxCollider2D>();

        // Guarda as posições originais dos obstáculos
        // para preservar o sistema de abertura vertical.
        if (topPipe != null && bottomPipe != null)
        {
            originalTopPosition = topPipe.localPosition;
            originalBottomPosition = bottomPipe.localPosition;
        }

        // Verifica se as imagens foram configuradas.
        if (obstacleSprites == null || obstacleSprites.Length == 0)
        {
            Debug.LogError(
                "ObstaclePair: nenhuma sprite configurada.",
                this
            );
            return;
        }

        // Sorteia uma imagem para cada obstáculo.
        // Os sorteios são independentes e podem repetir imagens.
        ConfigureRandomVisual(topVisual);
        ConfigureRandomVisual(bottomVisual);
    }

    private void ConfigureRandomVisual(SpriteRenderer visual)
    {
        // Verifica se a referência visual existe.
        if (visual == null)
        {
            Debug.LogError(
                "ObstaclePair: SpriteRenderer não configurado.",
                this
            );
            return;
        }

        // Recupera o PolygonCollider2D do mesmo objeto
        // responsável por mostrar a imagem.
        PolygonCollider2D polygonCollider =
            visual.GetComponent<PolygonCollider2D>();

        if (polygonCollider == null)
        {
            Debug.LogError(
                "ObstaclePair: PolygonCollider2D não encontrado.",
                visual
            );
            return;
        }

        // Escolhe aleatoriamente uma das imagens.
        int randomIndex = Random.Range(0, obstacleSprites.Length);

        Sprite selectedSprite = obstacleSprites[randomIndex];

        if (selectedSprite == null)
        {
            // Impede uma colisão com um formato antigo
            // caso alguma posição da lista esteja vazia.
            polygonCollider.enabled = false;

            Debug.LogError(
                "ObstaclePair: existe uma sprite vazia na lista.",
                this
            );
            return;
        }

        // Aplica a imagem sorteada ao SpriteRenderer.
        visual.sprite = selectedSprite;

        // Reconstrói o PolygonCollider2D utilizando
        // o formato físico salvo no Sprite Editor.
        bool colliderCreated =
            polygonCollider.CreateFromSprite(selectedSprite);

        if (!colliderCreated)
        {
            // Evita utilizar um contorno incorreto
            // caso a criação do collider falhe.
            polygonCollider.enabled = false;

            Debug.LogError(
                "ObstaclePair: falha ao criar o collider da sprite " +
                selectedSprite.name,
                this
            );

            return;
        }

        // Garante que o collider atualizado esteja ativo.
        polygonCollider.enabled = true;
    }

    public void Initialize(
        PlayerController player,
        GameManager manager)
    {
        // Mantém as referências usadas pela pontuação
        // e pelo controle do estado da partida.
        playerCollider = player.GetComponent<Collider2D>();
        gameManager = manager;
    }

    public void SetGap(float newGap)
    {
        if (topPipe == null || bottomPipe == null)
        {
            Debug.LogError(
                "ObstaclePair: referências dos obstáculos não configuradas.",
                this
            );
            return;
        }

        // Calcula quanto cada obstáculo precisa se
        // aproximar ou se afastar do centro da abertura.
        float adjustment = (originalGap - newGap) / 2f;

        Vector3 topPosition = originalTopPosition;
        Vector3 bottomPosition = originalBottomPosition;

        topPosition.y -= adjustment;
        bottomPosition.y += adjustment;

        // Altera somente as posições verticais.
        // Não modifica os tamanhos nem a velocidade.
        topPipe.localPosition = topPosition;
        bottomPipe.localPosition = bottomPosition;
    }

    private void FixedUpdate()
    {
        // Os obstáculos só se movimentam durante a gameplay.
        if (gameManager == null || !gameManager.IsPlaying)
            return;

        Vector2 newPosition = rb.position +
            Vector2.left * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);

        // Cada par concede somente um ponto quando
        // a referência de passagem ultrapassa o jogador.
        if (!hasScored &&
            obstacleCollider != null &&
            playerCollider != null)
        {
            if (obstacleCollider.bounds.max.x <
                playerCollider.bounds.min.x)
            {
                hasScored = true;
                gameManager.AddPoint();
            }
        }

        // Destrói o par quando sai da área de jogo.
        if (rb.position.x <= destroyX)
        {
            Destroy(gameObject);
        }
    }
}
