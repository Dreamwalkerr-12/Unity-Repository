using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Pickups : MonoBehaviour
{
    public float lifetime = 0.2f;
    public float moveSpeed = 3.0f;
    private GameObject player;

    // Abstract function to be implemented in subclasses
    public abstract void OnPickup(GameObject player);

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPickup(collision.gameObject);
            Destroy(gameObject, lifetime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPickup(collision.gameObject);
            Destroy(gameObject, lifetime);
        }
    }
}
