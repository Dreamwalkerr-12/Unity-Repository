using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField, Range(1, 20)] private float lifetime = 5.0f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private int maxBounces = 3;

    public bool isEnemyProjectile = false;

    private Rigidbody2D rb;
    private int currentBounces = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb.linearVelocity == Vector2.zero)
        {
            rb.linearVelocity = transform.right * speed;
        }

        Destroy(gameObject, lifetime); // Destroy if it lives too long
    }

    public void Launch(Vector2 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        else if (isEnemyProjectile && collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Die();
            }
            Destroy(gameObject);
        }
        else if (!isEnemyProjectile && collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (!collision.collider.isTrigger)
        {
            currentBounces++;

            if (currentBounces >= maxBounces)
            {
                Destroy(gameObject);
            }
        }
    }
}
