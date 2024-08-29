using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enemyState
{
    Wander,
    Follow,
    Die //basically useless now
}

[System.Serializable]
public class lootTableScript
{
    public GameObject itemPrefab;
    [Range(0, 100)] public float dropChance;
}

public class EnemyScript : MonoBehaviour
{
    GameObject player;
    public enemyState currentState = enemyState.Wander;

    public Transform target;
    private Rigidbody enemyRB;

    public int MaxHealth;
    [SerializeField] private int CurrentHealth;

    public float wanderSpeed = 2f;
    public float chasingSpeed = 8f;
    public float rotationSpeed = 2f;

    private float distance;
    public float viewingAngle = 45f;
    public float raycastHeight = 0.2f;

    [SerializeField] private float rayLength = 2f;

    private Vector3 wanderDirection;
    private Quaternion targetRotation;
    private bool canSeePlayer = false;
    [SerializeField] private float wanderTimer;
    [SerializeField] private float changeDirectionInterval = 3f; // Adjust as needed

    // Loot-related variables
    [Header("Loot")]
    public List<lootTableScript> LootTable = new List<lootTableScript>();

    private HealthManagerScript healthScript;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyRB = GetComponent<Rigidbody>();
        target = player.transform;

        CurrentHealth = MaxHealth;

        wanderDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        targetRotation = Quaternion.LookRotation(wanderDirection);

        healthScript = GetComponent<HealthManagerScript>();
    }

    void Update()
    {
        // Update the distance to the player
        distance = Vector3.Distance(transform.position, player.transform.position);

        // Check if the enemy can see the player
        Vector3 targetDirection = target.transform.position - transform.position;
        float angle = Vector3.Angle(targetDirection, transform.forward);
        Debug.DrawRay(transform.position, targetDirection, Color.green);

        if (angle <= viewingAngle && distance < 6)
        {
            canSeePlayer = true;
            print("Enemy sees player");
        }
        else
        {
            canSeePlayer = false;
        }

        // Switch states based on the enemy's current state and player visibility
        switch (currentState)
        {
            case enemyState.Wander:
                Wandering();
                break;
            case enemyState.Follow:
                Following();
                break;
        }

        if (canSeePlayer && currentState != enemyState.Die)
        {
            Debug.Log("Enemy switch to chasing state");
            currentState = enemyState.Follow;
        }
        else if (!canSeePlayer && currentState != enemyState.Die)
        {
            Debug.Log("Enemy switch to wandering state");
            currentState = enemyState.Wander;
        }
    }

    private void Wandering()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer > changeDirectionInterval)
        {
            // Choose a new random direction
            wanderDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            targetRotation = Quaternion.LookRotation(wanderDirection);
            wanderTimer = 0f; // Reset the timer
        }

        // Move in the current wander direction
        Vector3 horizontalVelocity = wanderDirection * wanderSpeed;
        enemyRB.velocity = new Vector3(horizontalVelocity.x, enemyRB.velocity.y, horizontalVelocity.z);

        Vector3 rayOrigin = transform.position + Vector3.up * raycastHeight; // Adjust if necessary to start from the middle of the enemy
        Vector3 rayDirection = wanderDirection;

        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

        // Check if the enemy hits a wall
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hitInfo, rayLength))
        {
            if (hitInfo.collider.CompareTag("Wall") || hitInfo.collider.CompareTag("Barrier"))
            {
                // Rotate to a new random direction
                wanderDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                targetRotation = Quaternion.LookRotation(wanderDirection);

                // Reset the timer so it doesn't immediately change direction again
                wanderTimer = 0f;
            }
        }

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void Following()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 horizontalVelocity = new Vector3(direction.x * chasingSpeed, enemyRB.velocity.y, direction.z * chasingSpeed);
        enemyRB.velocity = horizontalVelocity;

        // Smoothly rotate to face the player
        Vector3 targetDirection = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            TakeDamage(1); // Adjust the damage value as needed
        }
    }

    private void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Create a list to store items that are eligible for dropping based on their drop chance
        List<lootTableScript> eligibleItems = new List<lootTableScript>();

        // Populate the list with items whose drop chance is successful
        foreach (lootTableScript lootItem in LootTable)
        {
            if (Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                eligibleItems.Add(lootItem);
            }
        }

        // If there are eligible items, choose one randomly to drop
        if (eligibleItems.Count > 0)
        {
            // Choose a random item from the list of eligible items
            lootTableScript chosenItem = eligibleItems[Random.Range(0, eligibleItems.Count)];

            // Drop the chosen item
            InstantiateLoot(chosenItem.itemPrefab);
        }

        // Destroy the gameObject
        Destroy(gameObject);
    }

    void InstantiateLoot(GameObject loot)
    {
        if (loot != null)
        {
            GameObject droppedItemLoot = Instantiate(loot, transform.position, Quaternion.identity);
        }
    }
}
