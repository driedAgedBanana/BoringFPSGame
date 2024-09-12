using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmediateTeleportation : MonoBehaviour
{
    public GameObject playerObj;                  // The player object to be teleported
    public GameObject teleportationDestination;   // The destination where the player will be teleported

    private bool isPlayerInTriggerPad = false;    // Flag to check if player is on the teleport pad

    public AudioClip teleportingClip;             // Audio clip for teleportation sound
    private AudioSource teleportingSFX;           // AudioSource component for playing the teleport sound

    [SerializeField] private RadioManager radio;  // Reference to the RadioManager script to stop music

    // Start is called before the first frame update
    void Start()
    {
        // Get the AudioSource component and configure it
        teleportingSFX = GetComponent<AudioSource>();
        teleportingSFX.playOnAwake = false;
        teleportingSFX.clip = teleportingClip;
        teleportingSFX.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is on the trigger pad and the teleportation destination is assigned
        if (isPlayerInTriggerPad && teleportationDestination != null)
        {
            Debug.Log("Player touched the immediate teleportation pad!");

            // Teleport the player to the destination
            playerObj.transform.position = teleportationDestination.transform.position;

            // Play the teleporting sound effect
            teleportingSFX.Play();

            // Stop the radio music when teleporting
            radio.StopMusic();
        }
        else if (isPlayerInTriggerPad && teleportationDestination == null)
        {
            Debug.LogWarning("Teleportation destination is not assigned!");
        }
    }

    // Called when the player enters the trigger area
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerObj)
        {
            isPlayerInTriggerPad = true;  // Mark the player as on the trigger pad
        }
    }

    // Called when the player exits the trigger area
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerObj)
        {
            isPlayerInTriggerPad = false;  // Mark the player as off the trigger pad
        }
    }
}
