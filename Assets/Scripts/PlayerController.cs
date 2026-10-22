
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameManager gameManager;

    [Header("Movimentação")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float gravityScale = 1.5f;

    private Rigidbody2D rb;
    private bool gameStarted = false;

    public bool IsGameStarted => gameStarted;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Mantém o personagem parado até o primeiro clique.
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        // Impede novos comandos após o Game Over.
        if (gameManager == null || gameManager.IsGameOver)
            return;

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

    private void Jump()
    {
        // Aplica sempre a mesma velocidade vertical.
        rb.linearVelocity = new Vector2(0f, jumpForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameStarted || gameManager == null)
            return;

        // Nesta fase, qualquer colisão física encerra a partida.
        gameManager.EndGame();
    }
}
