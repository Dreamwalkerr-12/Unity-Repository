using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Koopa : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float walkSpeed = 2f;
    public float shellSpeed = 10f;
    public bool IsShell => isShell;
    public bool IsSliding => isSliding;

    private Rigidbody2D rb;
    private Vector3 targetPoint;
    private bool isShell = false;
    private bool isSliding = false;
    private bool isDead = false;
    private Animator animator;

    private Vector3 originalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        targetPoint = pointB.position;
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isDead) return;

        if (!isShell)
        {
            Patrol();
        }
        else if (isSliding)
        {
            // Keep sliding in the same direction
            float direction = transform.localScale.x > 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * shellSpeed, rb.linearVelocity.y);
        }
    }

    void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint, walkSpeed * Time.deltaTime);

        Vector3 scale = originalScale;
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

        GameObject other = collision.gameObject;

        if (other.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f) // player is above
                {
                    if (!isShell)
                    {
                        EnterShell();
                        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
                        if (playerRb != null)
                            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 10f);
                    }
                    else if (!isSliding)
                    {
                        // Kick shell
                        float direction = other.transform.position.x < transform.position.x ? 1f : -1f;
                        StartSliding(direction);
                        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
                        if (playerRb != null)
                            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 10f);
                    }
                    return;
                }
            }

            if (isShell && isSliding)
            {
                Debug.Log("Player hit by sliding shell – should take damage.");
            }
        }
        else if (other.CompareTag("Projectile"))
        {
            Die();
            Destroy(other);
        }
        else if (isShell && isSliding && other.CompareTag("Enemy"))
        {
            Destroy(other);
        }
        else if (isShell && isSliding && other.CompareTag("Wall"))
        {
            // Bounce off wall
            BounceShell();
        }
    }


    void EnterShell()
    {
        isShell = true;
        isSliding = false;
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetTrigger("EnterShellTrigger");
        }

        Debug.Log("Koopa entered shell state.");
    }

    void StartSliding(float direction)
    {
        isSliding = true;

        // Set direction
        transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * direction, transform.localScale.y, originalScale.z);

        // Set animation
        if (animator != null)
        {
            animator.SetBool("IsSliding", true);
        }

        Debug.Log("Koopa shell started sliding.");
    }


    void BounceShell()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f; // Flip direction
        transform.localScale = scale;

        float direction = scale.x > 0 ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * shellSpeed, rb.linearVelocity.y);

        Debug.Log("Shell bounced off wall.");
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
