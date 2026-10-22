
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimentação")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float gravityScale = 1.5f;

    private Rigidbody2D rb;
    private bool gameStarted = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // O personagem começa imóvel, aguardando o primeiro clique.
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        bool mouseClick = Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;

        bool spacePressed = Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame;

        // Somente o primeiro clique inicia a partida.
        if (!gameStarted)
        {
            if (mouseClick)
            {
                gameStarted = true;
                rb.gravityScale = gravityScale;
                Jump();
            }

            return;
        }

        // Após iniciar, clique ou Espaço aplica outro impulso.
        if (mouseClick || spacePressed)
        {
            Jump();
        }
    }

    private void Jump()
    {
        // Substitui a velocidade vertical para manter
        // os impulsos consistentes, mesmo durante a queda.
        rb.linearVelocity = new Vector2(0f, jumpForce);
    }
}
