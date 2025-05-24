using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
    public float attackRadius = 5f;
    public float fireCooldown = 2f;
    public GameObject projectilePrefab;
    public Transform firePoint; // Assign in Inspector
    public LayerMask playerLayer;

    private float nextFireTime = 0f;
    private Transform player;
    private SpriteRenderer sr;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (firePoint == null)
            firePoint = transform;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (player == null) return;

        FacePlayer();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRadius && Time.time >= nextFireTime)
        {
            FireAtPlayer();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    void FacePlayer()
    {
        if (sr != null)
        {
            // Flip sprite based on player's position
            sr.flipX = (player.position.x < transform.position.x);
        }
    }

    void FireAtPlayer()
    {
        if (projectilePrefab == null) return;

        // Calculate horizontal-only direction (left or right)
        float horizontalDirection = player.position.x < transform.position.x ? -1f : 1f;
        Vector2 direction = new Vector2(horizontalDirection, 0f);

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.isEnemyProjectile = true;
            projectile.Launch(direction);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
