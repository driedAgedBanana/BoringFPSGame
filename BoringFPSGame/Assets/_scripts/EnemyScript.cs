using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enemyState
{
    Wander,
    Follow,
    Die
}

public class EnemyScript : MonoBehaviour
{
    GameObject player;
    public enemyState currentState = enemyState.Wander;

    public Transform target;
    private Rigidbody enemyRB;

    public float wanderSpeed = 2f;
    public float chasingSpeed = 8f;
    public float range = 2.1f;
    public float rotationSpeed = 2f; // Adjust this for smoother/slower rotations

    private Vector3 wanderDirection;
    private Quaternion targetRotation;
    [SerializeField] private float wanderTimer;
    [SerializeField] private float changeDirectionInterval = 3f; // Adjust as needed

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyRB = GetComponent<Rigidbody>();
        target = player.transform;

        wanderDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        targetRotation = Quaternion.LookRotation(wanderDirection);
    }

    void Update()
    {
        switch (currentState)
        {
            case enemyState.Wander:
                Wandering();
                break;
            case enemyState.Follow:
                Following();
                break;
            case enemyState.Die:
                // No die yet :(
                break;
        }

        if (isPlayerInRange(range) && currentState != enemyState.Die)
        {
            currentState = enemyState.Follow;
        }
        else if (!isPlayerInRange(range) && currentState != enemyState.Die)
        {
            currentState = enemyState.Wander;
        }
    }

    private bool isPlayerInRange(float range)
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
            targetRotation = Quaternion.LookRotation(wanderDirection);
            wanderTimer = 0f; // Reset the timer
        }

        // Move in the current wander direction
        Vector3 horizontalVelocity = wanderDirection * wanderSpeed;
        enemyRB.velocity = new Vector3(horizontalVelocity.x, enemyRB.velocity.y, horizontalVelocity.z);

        Vector3 rayOrigin = transform.position + Vector3.up * 0.2f; // Adjust if necessary to start from the middle of the enemy
        Vector3 rayDirection = wanderDirection;
        float rayLength = 2f;

        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

        // Check if the enemy hits a wall
        if (Physics.Raycast(rayOrigin, rayDirection, rayLength))
        {
            Debug.Log("Enemy hit a wall!");

            // Rotate to a new random direction
            wanderDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            targetRotation = Quaternion.LookRotation(wanderDirection);

            // Reset the timer so it doesn't immediately change direction again
            wanderTimer = 0f;
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
}
