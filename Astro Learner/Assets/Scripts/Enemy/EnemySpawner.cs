using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f; // Time between spawns
    public float spawnRangeX = 5f;   // Range for X-coordinate spawning
    public float minSpawnDistance = 2f; // Minimum distance between enemy spawns

    private Camera mainCamera;
    private List<Vector3> recentSpawnPositions = new List<Vector3>();
    private int maxTrackedPositions = 10; // Limit the number of tracked positions to optimize performance

    // Added: List of movement types for randomization
    private EnemyShip.MovementType[] movementTypes = {
        EnemyShip.MovementType.Straight,
        EnemyShip.MovementType.Zigzag,
        EnemyShip.MovementType.Circular
    };

    // Added: Optional selection of a specific movement type
    public EnemyShip.MovementType? selectedMovementType = null;

    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();

            if (spawnPosition == Vector3.zero)
            {
                // If no valid position is found after multiple attempts, bypass validation
                Debug.LogWarning("Could not find a valid position. Spawning without validation.");
                spawnPosition = GenerateSpawnPosition();
            }

            // Instantiate the enemy
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            // Added: Assign a random or selected movement type
            EnemyShip.MovementType movementTypeToAssign = selectedMovementType.HasValue
                ? selectedMovementType.Value
                : movementTypes[Random.Range(0, movementTypes.Length)];
            EnemyShip enemyShip = enemyInstance.GetComponent<EnemyShip>();
            if (enemyShip != null)
            {
                enemyShip.SetMovementType(movementTypeToAssign);
                Debug.Log($"Enemy spawned with movement type: {movementTypeToAssign}");
            }

            recentSpawnPositions.Add(spawnPosition);

            // Limit tracked positions to optimize memory usage
            if (recentSpawnPositions.Count > maxTrackedPositions)
            {
                recentSpawnPositions.RemoveAt(0);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        for (int attempts = 0; attempts < 10; attempts++) // Limit the number of attempts
        {
            Vector3 potentialPosition = GenerateSpawnPosition();

            if (IsPositionValid(potentialPosition))
            {
                return potentialPosition;
            }
        }

        return Vector3.zero; // Return zero vector if no valid position is found
    }

    private Vector3 GenerateSpawnPosition()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float cameraTopY = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f)).y;
        float spawnY = cameraTopY + 1f; // Spawn above the camera
        return new Vector3(randomX, spawnY, 0f);
    }

    private bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 recentPosition in recentSpawnPositions)
        {
            if (Vector3.Distance(position, recentPosition) < minSpawnDistance)
            {
                return false; // Too close to another enemy
            }
        }
        return true;
    }
}
