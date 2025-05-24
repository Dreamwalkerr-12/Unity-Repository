using UnityEngine;

public class Life : Pickups
{
    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = new Vector2(-2, 2);
    }

    private void Update()
    {
        rb.linearVelocity = new Vector2(-2, rb.linearVelocity.y);
    }

    public override void OnPickup(GameObject player) => player.GetComponent<PlayerController>().Lives++;
}
