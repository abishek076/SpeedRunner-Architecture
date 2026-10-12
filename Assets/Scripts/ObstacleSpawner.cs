using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnRate = 1.5f; // Spawn an obstacle every 1.5 seconds
    private float timer = 0f;

    [Header("Location Settings")]
    public float spawnZ = 30f; // Spawn far in the distance
    public float laneDistance = 2.0f; // MUST match your PlayerMovement laneDistance

    void Update()
    {
        // A standard game development timer using Time.deltaTime
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnObstacle();
            timer = spawnRate; // Reset the timer
        }
    }

    private void SpawnObstacle()
    {
        // 1. Ask the Object Pooler for an available clone
        GameObject obstacle = ObjectPooler.SharedInstance.GetPooledObject();

        if (obstacle != null)
        {
            // 2. Pick a random lane: 0 (Left), 1 (Center), or 2 (Right)
            // Note: Random.Range for integers is exclusive on the max value, so (0, 3) returns 0, 1, or 2.
            int randomLane = Random.Range(0, 3);

            // 3. Calculate the exact X position using our grid math
            float targetX = (randomLane - 1) * laneDistance;

            // 4. Position the obstacle and turn it on! (Y = 0.5f ensures it sits on top of the ground)
            obstacle.transform.position = new Vector3(targetX, 0.5f, spawnZ);
            obstacle.SetActive(true);
        }
    }
}
