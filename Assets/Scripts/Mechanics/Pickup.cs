using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        Life,
        Powerup,
        Score
    }

    public PickupType pickupType;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController pc = collision.GetComponent<PlayerController>();
            Shoot shoot = collision.GetComponent<Shoot>();
            GroundCheck gc = collision.GetComponent<GroundCheck>();


            switch (pickupType)
            {
                case PickupType.Life:
                    pc.Lives++; //increment player lives
                    break;
                case PickupType.Powerup:
                    pc.SpeedChange(); //call the speed change method
                    break;
                case PickupType.Score:
                    //pc.Score++;
                    break;
            }
            Destroy(gameObject);
        }
    }
}
