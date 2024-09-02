using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Wander,
    Follow,
    Search,
    Hide,
    Die
}

[System.Serializable]
public class LootTableScript
{
    public GameObject itemPrefab;
    [Range(0, 100)] public float dropChance;
}

public class EnemyScript : MonoBehaviour
{
    GameObject player;
    public EnemyState currentState = EnemyState.Wander;

    private NavMeshAgent navAgent;
    public int MaxHealth;
    [SerializeField] private int CurrentHealth;

    private float distance;
    public float viewingAngle = 85f;
    public float chaseDistance = 8f;

    private bool canSeePlayer = false;
    private Vector3 lastKnownPlayerPosition;
    private Vector3 hidingSpot;

    [Header("Loot")]
    public List<LootTableScript> LootTable = new List<LootTableScript>();

    public float quickTurnSpeed = 20f; // Speed at which the enemy turns towards the player
    public float anticipationFactor = 1.5f; // Factor by which the enemy anticipates the player's movement

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        navAgent = GetComponent<NavMeshAgent>();

        CurrentHealth = MaxHealth;
        currentState = EnemyState.Wander;
        navAgent.speed = 5f;
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);
        Vector3 targetDirection = player.transform.position - transform.position;
        float angle = Vector3.Angle(targetDirection, transform.forward);

        if (angle <= viewingAngle && distance < chaseDistance)
        {
            canSeePlayer = true;
            lastKnownPlayerPosition = player.transform.position;
        }
        else
        {
            canSeePlayer = false;
        }

        switch (currentState)
        {
            case EnemyState.Wander:
                Wander();
                break;
            case EnemyState.Follow:
                Follow();
                break;
            case EnemyState.Search:
                Search();
                break;
            case EnemyState.Hide:
                Hide();
                break;
        }

        if (canSeePlayer && currentState != EnemyState.Die)
        {
            currentState = EnemyState.Follow;
            QuicklyTurnTowardsPlayer();  // Rapidly turn towards the player
        }
        else if (!canSeePlayer && currentState != EnemyState.Die)
        {
            if (currentState == EnemyState.Follow)
            {
                currentState = EnemyState.Search;
                IdentifyHidingSpot();
            }
            else if (currentState == EnemyState.Search && !canSeePlayer)
            {
                currentState = EnemyState.Hide;
            }
            else
            {
                currentState = EnemyState.Wander;
            }
        }
    }

    private void QuicklyTurnTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * quickTurnSpeed);
    }

    private void Wander()
    {
        if (!navAgent.hasPath)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += transform.position;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, 10f, NavMesh.AllAreas);
            navAgent.SetDestination(navHit.position);
        }
    }

    private void Follow()
    {
        navAgent.speed = 10f; // Increased speed during chase

        // Anticipate the player's movement by slightly adjusting the target position
        Vector3 anticipatedPosition = player.transform.position + player.GetComponent<Rigidbody>().velocity * anticipationFactor;
        navAgent.SetDestination(anticipatedPosition);
    }

    private void Search()
    {
        navAgent.speed = 6f;
        navAgent.SetDestination(lastKnownPlayerPosition);

        if (navAgent.remainingDistance < 1f && !canSeePlayer)
        {
            currentState = EnemyState.Hide;
        }
    }

    private void Hide()
    {
        if (hidingSpot != Vector3.zero)
        {
            navAgent.speed = 5f;
            navAgent.SetDestination(hidingSpot);

            if (navAgent.remainingDistance < 1f)
            {
                currentState = EnemyState.Wander;
            }
        }
    }

    private void IdentifyHidingSpot()
    {
        hidingSpot = transform.position + Random.insideUnitSphere * 5f;
        NavMeshHit navHit;
        NavMesh.SamplePosition(hidingSpot, out navHit, 5f, NavMesh.AllAreas);
        hidingSpot = navHit.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        List<LootTableScript> eligibleItems = new List<LootTableScript>();

        foreach (LootTableScript lootItem in LootTable)
        {
            if (Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                eligibleItems.Add(lootItem);
            }
        }

        if (eligibleItems.Count > 0)
        {
            LootTableScript chosenItem = eligibleItems[Random.Range(0, eligibleItems.Count)];
            InstantiateLoot(chosenItem.itemPrefab);
        }

        Destroy(gameObject);
    }

    void InstantiateLoot(GameObject loot)
    {
        if (loot != null)
        {
            Instantiate(loot, transform.position, Quaternion.identity);
        }
    }
}
