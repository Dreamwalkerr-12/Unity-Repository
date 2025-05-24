using UnityEngine;

public class SpeedPickup : Pickups
{
    public override void OnPickup(GameObject player)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.SpeedChange();
        }
    }
}