using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField, Range(1, 20)] private float lifetime = 1.0f;
    [SerializeField] private float speed = 10f;

    public bool isEnemyProjectile = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // If velocity not set manually, default to right
        if (rb.linearVelocity == Vector2.zero)
        {
            rb.linearVelocity = transform.right * speed;
        }

        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Launches the projectile in a specified direction.
    /// </summary>
    /// <param name="direction">Normalized 2D direction vector</param>
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
                player.Die(); // Or call player.TakeDamage()
            }
            Destroy(gameObject);
        }
        else if (!isEnemyProjectile && collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject); // Or call enemy.Die()
            Destroy(gameObject);
        }
        else if (!collision.collider.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
