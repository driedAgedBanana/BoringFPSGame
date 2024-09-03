using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    public Transform markerA;  // Assign the Marker A (start point) in the Inspector
    public Transform markerB;  // Assign the Marker B (end point) in the Inspector
    public float speed = 2.0f;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 targetPosition;

    void Start()
    {
        if (markerA == null || markerB == null)
        {
            Debug.LogError("Markers A and B must be assigned in the Inspector!");
            return;
        }

        // Capture the initial positions of the markers
        pointA = markerA.position;
        pointB = markerB.position;

        targetPosition = pointB;  // Set the initial target position to Marker B's initial position
    }

    void Update()
    {
        // Move the wall towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // If the wall reaches the target position, switch the target
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            targetPosition = targetPosition == pointA ? pointB : pointA;
        }
    }
}
