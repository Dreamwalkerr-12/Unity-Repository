using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Goomba : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector3 targetPoint;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPoint = pointB.position;
    }

    void Update()
    {
        if (isDead) return;

        Patrol();
    }

    void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);

        // Flip sprite direction
        Vector3 scale = transform.localScale;
        if (targetPoint.x > transform.position.x)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;

        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            targetPoint = (targetPoint == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    Die();
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 10f); // bounce
                    }
                    return;
                }
            }

            // Otherwise, hurt the player (not implemented)
            Debug.Log("Player hit from side - should take damage.");
        }
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            Die();
            Destroy(collision.gameObject); // optional: destroy the projectile
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = Color.gray;
        Destroy(gameObject, 0.5f);
    }
}
