
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameManager gameManager;
    
    [SerializeField] private GameSFXController gameSFXController;

    [Header("Movimentação")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float gravityScale = 1.5f;

    [Header("Limite Superior")]
    [SerializeField] private float topMargin = 0.1f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Camera gameCamera;

    private bool gameStarted = false;

    public bool IsGameStarted => gameStarted;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        playerCollider = GetComponent<Collider2D>();
        gameCamera = Camera.main;

        // O personagem começa imóvel até o primeiro clique.
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        // Bloqueia os comandos durante o Pause ou Game Over.
        if (gameManager == null ||
            gameManager.IsPaused ||
            gameManager.IsGameOver)
        {
            return;
        }

        bool mouseClick = Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;

        bool spacePressed = Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame;

        if (!gameStarted)
        {
            if (mouseClick)
            {
                gameStarted = true;

                gameManager.StartGame();

                rb.gravityScale = gravityScale;
                Jump();
            }

            return;
        }

        if (mouseClick || spacePressed)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        // Verifica o limite somente durante a partida.
        if (gameManager == null ||
            !gameManager.IsPlaying ||
            gameCamera == null ||
            playerCollider == null)
        {
            return;
        }

        // Calcula o topo visível da câmera ortográfica.
        float cameraTop = gameCamera.transform.position.y +
            gameCamera.orthographicSize - topMargin;

        // Mede quanto o collider ultrapassou esse limite.
        float excessHeight = playerCollider.bounds.max.y - cameraTop;

        if (excessHeight > 0f)
        {
            // Reposiciona o jogador dentro da área visível.
            rb.position = new Vector2(
                rb.position.x,
                rb.position.y - excessHeight
            );
        }

        // Impede que o jogador continue subindo no limite.
        if (excessHeight >= 0f && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                0f
            );
        }
    }

    
    private void Jump()
    {
        // Aplica o impulso vertical.
        rb.linearVelocity = new Vector2(0f, jumpForce);

        // Reproduz o efeito a cada pulo.
        if (gameSFXController != null)
        {
            gameSFXController.PlayJump();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameStarted || gameManager == null)
            return;

        // Colisões com o chão ou obstáculos causam Game Over.
        gameManager.EndGame();
    }
}
