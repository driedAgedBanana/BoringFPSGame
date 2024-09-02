using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Changed from enemySpawner to enemyPrefab

    [SerializeField] private float spawnInterval = 3f;
    private float timer;

    public int maximumEnemies = 20;

    void Update()
    {
        // Update timer
        timer += Time.deltaTime;

        // Check if it's time to spawn a new enemy and if the number of active enemies is below the maximum
        if (timer >= spawnInterval && GetActiveEnemyCount() < maximumEnemies)
        {
            // Reset the timer
            timer = 0f;

            // Instantiate a new enemy
            Instantiate(enemyPrefab, transform.position, transform.rotation);
        }
    }

    // Method to count active enemies
    private int GetActiveEnemyCount()
    {
        // Find all objects with the "Enemy" tag and return their count
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemies");
        return enemies.Length;
    }
}
