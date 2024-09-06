using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    public GameObject playerObj;
    public GameObject teleportationDestination;

    private bool isPlayerInTrigger = false;

    public AudioClip audioSource;
    private AudioSource teleportingSFX;

    private void Start()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("You haven't assigned the SFX through the inspector!");
            this.enabled = false;
        }

        teleportingSFX = GetComponent<AudioSource>();
        teleportingSFX.playOnAwake = false;
        teleportingSFX.clip = audioSource;
        teleportingSFX.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Player pressed F");
            if (teleportationDestination != null)
            {
                // Debug.Log("Current Player Position: " + playerObj.transform.position);
                // Debug.Log("Teleportation Destination Position: " + teleportationDestination.transform.position);

                playerObj.transform.position = teleportationDestination.transform.position;
                teleportingSFX.Play();

                // Debug.Log("Player teleported to: " + playerObj.transform.position);
            }
            else
            {
                Debug.LogError("Teleportation destination is not assigned.");
                teleportingSFX.Stop();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerObj)
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerObj)
        {
            isPlayerInTrigger = false;
        }
    }
}
