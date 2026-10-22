
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

    private Rigidbody2D rb;
    private Collider2D obstacleCollider;
    private Collider2D playerCollider;
    private GameManager gameManager;

    private Vector3 originalTopPosition;
    private Vector3 originalBottomPosition;

    private bool hasScored = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        obstacleCollider = GetComponentInChildren<BoxCollider2D>();

        // Guarda as posições originais dos canos.
        // Elas serão usadas para ajustar a abertura.
        if (topPipe != null && bottomPipe != null)
        {
            originalTopPosition = topPipe.localPosition;
            originalBottomPosition = bottomPipe.localPosition;
        }
    }

    public void Initialize(
        PlayerController player,
        GameManager manager)
    {
        playerCollider = player.GetComponent<Collider2D>();
        gameManager = manager;
    }

    public void SetGap(float newGap)
    {
        if (topPipe == null || bottomPipe == null)
        {
            Debug.LogError(
                "ObstaclePair: referências dos canos não configuradas.",
                this
            );
            return;
        }

        // Calcula quanto cada cano precisa se aproximar
        // ou se afastar do centro da abertura.
        float adjustment = (originalGap - newGap) / 2f;

        Vector3 topPosition = originalTopPosition;
        Vector3 bottomPosition = originalBottomPosition;

        topPosition.y -= adjustment;
        bottomPosition.y += adjustment;

        // Altera apenas a posição vertical dos canos.
        // O tamanho e a largura continuam iguais.
        topPipe.localPosition = topPosition;
        bottomPipe.localPosition = bottomPosition;
    }

    private void FixedUpdate()
    {
        if (gameManager == null || !gameManager.IsPlaying)
            return;

        Vector2 newPosition = rb.position +
            Vector2.left * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);

        // Cada par concede apenas um ponto quando
        // passa completamente pelo jogador.
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

        // Remove obstáculos que saíram da tela.
        if (rb.position.x <= destroyX)
        {
            Destroy(gameObject);
        }
    }
}
