using System.Collections;
using UnityEngine;

public enum EnemyState
{
    Wander,
    Follow,
    Die
}

public class EnemyScript : MonoBehaviour
{
    GameObject player;
    public EnemyState currentState = EnemyState.Wander;

    public Transform target;
    private Rigidbody enemyRB;

    public float moveSpeed = 2f;
    public float range = 2.1f;

    private Vector3 wanderDirection;
    private float wanderTimer = 0f;
    public float changeDirectionInterval = 3f; // Change direction every 3 seconds

    Ray ray;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyRB = GetComponent<Rigidbody>();
        target = player.transform;
        StartCoroutine(WanderRoutine());
    }

    void Update()
    { 
        switch (currentState)
        {
            case EnemyState.Wander:
                Wandering();
                break;
            case EnemyState.Follow:
                Following();
                break;
            case EnemyState.Die:
                // No die yet :(
                break;
        }

        if (IsPlayerInRange(range) && currentState != EnemyState.Die)
        {
            currentState = EnemyState.Follow;
        }
        else if (!IsPlayerInRange(range) && currentState != EnemyState.Die)
        {
            currentState = EnemyState.Wander;
        }
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    private void Wandering()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer > changeDirectionInterval)
        {
            // Choose a new random direction
            wanderDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            wanderTimer = 0f; // Reset the timer
        }

        // Move in the current wander direction
        enemyRB.velocity = wanderDirection * moveSpeed;

        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; // Adjust if necessary to start from the middle of the enemy
        Vector3 rayDirection = wanderDirection;
        float rayLength = 2f;

        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

        // Check if the enemy hits a wall
        if (Physics.Raycast(rayOrigin, rayDirection, rayLength))
        {
            Debug.Log("Enemy hit a wall!");

            // Rotate to a new random direction
            wanderDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

            // Optionally, you can also change the enemy's rotation to face the new direction
            transform.rotation = Quaternion.LookRotation(wanderDirection);

            // Reset the timer so it doesn't immediately change direction again
            wanderTimer = 0f;
        }
    }

    private void Following()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        enemyRB.velocity = new Vector3(direction.x * moveSpeed, enemyRB.velocity.y, direction.z * moveSpeed);

        // Facing the player
        Vector3 targetDirection = target.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, moveSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            wanderDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            yield return new WaitForSeconds(changeDirectionInterval);
        }
    }
}
