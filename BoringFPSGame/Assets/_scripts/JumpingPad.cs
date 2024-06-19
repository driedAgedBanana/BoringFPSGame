using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingPad : MonoBehaviour
{
    public float BouncingForce = 12f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called with: " + other.name); // Log the name of the object that entered the trigger

        if (other.CompareTag("Player"))
        {
            Rigidbody playerRB = other.GetComponentInParent<Rigidbody>();

            if (playerRB != null)
            {
                playerRB.AddForce(Vector3.up * BouncingForce, ForceMode.Impulse);
                Debug.Log("Touching the jump pad!");
            }
            else
            {
                Debug.LogWarning("Boy you forgot to assign the rigidbody to the player");
            }
        }
    }
}
