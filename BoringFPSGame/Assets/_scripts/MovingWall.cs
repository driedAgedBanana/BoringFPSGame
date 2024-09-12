using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    public Transform markerA;  // Assign the Marker A (start point) in the Inspector
    public Transform markerB;  // Assign the Marker B (end point) in the Inspector
    public float speed = 2.0f; // Speed at which the wall moves

    private Vector3 pointA;   // Position of Marker A
    private Vector3 pointB;   // Position of Marker B
    private Vector3 targetPosition; // Current target position for the wall to move towards

    void Start()
    {
        // Check if markers are assigned in the Inspector
        if (markerA == null || markerB == null)
        {
            Debug.LogError("Markers A and B must be assigned in the Inspector!");
            return;
        }

        // Capture the positions of Marker A and Marker B
        pointA = markerA.position;
        pointB = markerB.position;

        targetPosition = pointB;  // Set the initial target position to Marker B
    }

    void Update()
    {
        // Move the wall towards the current target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the wall has reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // Switch the target position between Marker A and Marker B
            targetPosition = targetPosition == pointA ? pointB : pointA;
        }
    }
}
