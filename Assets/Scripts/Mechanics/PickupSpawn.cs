using UnityEngine;

public class PickupSpawn : MonoBehaviour
{
    public GameObject[] pickupPrefabs;  // List of collectible prefabs
    public int numberOfSpawns = 5;      // Number of collectibles to spawn
    public float spawnRadius = 3f;      // Radius around the center for spawning

    private void Start()
    {
        if (pickupPrefabs.Length == 0) return;

        // Select a random collectible prefab
        GameObject selectedPickup = pickupPrefabs[Random.Range(0, pickupPrefabs.Length)];

        // Spawn `numberOfSpawns` instances around the spawn point
        for (int i = 0; i < numberOfSpawns; i++)
        {
            Vector2 spawnOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(spawnOffset.x, spawnOffset.y, 0);
            Instantiate(selectedPickup, spawnPosition, Quaternion.identity);
        }
    }
}