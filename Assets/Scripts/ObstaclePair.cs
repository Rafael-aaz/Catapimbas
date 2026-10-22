
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ObstaclePair : MonoBehaviour
{
    [Header("Movimentação")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Destruição")]
    [SerializeField] private float destroyX = -11f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Move o conjunto inteiro para a esquerda.
        Vector2 newPosition = rb.position +
            Vector2.left * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);

        // Remove o par após sair da área visível.
        if (rb.position.x <= destroyX)
        {
            Destroy(gameObject);
        }
    }
}
