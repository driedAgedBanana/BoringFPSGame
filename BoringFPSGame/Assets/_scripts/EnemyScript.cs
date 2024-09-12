using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Enumeration for enemy states
public enum EnemyState
{
    Wander,
    Follow,
    Search,
    Hide,
    Die
}

// Serializable class for loot table entries
[System.Serializable]
public class LootTableScript
{
    public GameObject itemPrefab; // The item to drop
    [Range(0, 100)] public float dropChance; // Chance of dropping the item
}

public class EnemyScript : BaseLootDrop
{
    GameObject player; // Reference to the player object
    public EnemyState currentState = EnemyState.Wander; // Current state of the enemy

    private NavMeshAgent navAgent; // Navigation agent for pathfinding
    public int MaxHealth; // Maximum health of the enemy
    [SerializeField] private int CurrentHealth; // Current health of the enemy

    private float distance; // Distance between the enemy and the player
    public float viewingAngle = 85f; // Field of view angle
    public float chaseDistance = 8f; // Distance at which the enemy starts chasing

    private bool canSeePlayer = false; // Whether the enemy can see the player
    private Vector3 lastKnownPlayerPosition; // Last known position of the player
    private Vector3 hidingSpot; // Location the enemy will hide at

    public float quickTurnSpeed = 20f; // Speed of quick turning towards the player
    public float anticipationFactor = 1.5f; // Factor to predict player's movement

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player object
        navAgent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component

        CurrentHealth = MaxHealth; // Initialize current health
        currentState = EnemyState.Wander; // Set initial state
        navAgent.speed = 5f; // Set default speed
    }

    void Update()
    {
        // Calculate distance and angle to the player
        distance = Vector3.Distance(transform.position, player.transform.position);
        Vector3 targetDirection = player.transform.position - transform.position;
        float angle = Vector3.Angle(targetDirection, transform.forward);

        // Check if the player is within the enemy's view and distance
        if (angle <= viewingAngle && distance < chaseDistance)
        {
            canSeePlayer = true;
            lastKnownPlayerPosition = player.transform.position;
        }
        else
        {
            canSeePlayer = false;
        }

        // Handle state transitions
        switch (currentState)
        {
            case EnemyState.Wander:
                Wander(); // Handle wandering behavior
                break;
            case EnemyState.Follow:
                Follow(); // Handle following behavior
                break;
            case EnemyState.Search:
                Search(); // Handle searching behavior
                break;
            case EnemyState.Hide:
                Hide(); // Handle hiding behavior
                break;
        }

        // State transitions based on player visibility
        if (canSeePlayer && currentState != EnemyState.Die)
        {
            currentState = EnemyState.Follow; // Switch to follow state
            QuicklyTurnTowardsPlayer(); // Rapidly turn towards the player
        }
        else if (!canSeePlayer && currentState != EnemyState.Die)
        {
            if (currentState == EnemyState.Follow)
            {
                currentState = EnemyState.Search; // Switch to search state
                IdentifyHidingSpot(); // Find a hiding spot
            }
            else if (currentState == EnemyState.Search && !canSeePlayer)
            {
                currentState = EnemyState.Hide; // Switch to hide state
            }
            else
            {
                currentState = EnemyState.Wander; // Default to wandering
            }
        }
    }

    // Rapidly turn the enemy towards the player
    private void QuicklyTurnTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * quickTurnSpeed);
    }

    // Handle wandering behavior
    private void Wander()
    {
        if (!navAgent.hasPath)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 10f; // Random direction
            randomDirection += transform.position;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, 10f, NavMesh.AllAreas); // Find a valid position
            navAgent.SetDestination(navHit.position); // Set destination for wandering
        }
    }

    // Handle following behavior
    private void Follow()
    {
        navAgent.speed = 10f; // Increased speed during chase

        // Anticipate the player's future position
        Vector3 anticipatedPosition = player.transform.position + player.GetComponent<Rigidbody>().velocity * anticipationFactor;
        navAgent.SetDestination(anticipatedPosition); // Set destination to anticipated position
    }

    // Handle searching behavior
    private void Search()
    {
        navAgent.speed = 6f; // Moderate speed during search
        navAgent.SetDestination(lastKnownPlayerPosition); // Move to last known player position

        // Switch to hide state if the search is complete and player is not visible
        if (navAgent.remainingDistance < 1f && !canSeePlayer)
        {
            currentState = EnemyState.Hide;
        }
    }

    // Handle hiding behavior
    private void Hide()
    {
        if (hidingSpot != Vector3.zero)
        {
            navAgent.speed = 5f; // Normal speed during hide
            navAgent.SetDestination(hidingSpot); // Move to hiding spot

            // Switch to wander state once hiding is complete
            if (navAgent.remainingDistance < 1f)
            {
                currentState = EnemyState.Wander;
            }
        }
    }

    // Identify a suitable hiding spot
    private void IdentifyHidingSpot()
    {
        hidingSpot = transform.position + Random.insideUnitSphere * 5f; // Random hiding spot
        NavMeshHit navHit;
        NavMesh.SamplePosition(hidingSpot, out navHit, 5f, NavMesh.AllAreas); // Find a valid hiding position
        hidingSpot = navHit.position; // Update hiding spot
    }

    // Handle collision with bullets
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(1); // Apply damage from bullet
        }
    }

    // Apply damage to the enemy
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage; // Decrease health

        if (CurrentHealth <= 0)
        {
            Die(); // Handle death
        }
    }

    // Handle enemy death
    private void Die()
    {
        DropLoot(); // Drop loot upon death
        Destroy(gameObject); // Remove the enemy from the game
    }
}
