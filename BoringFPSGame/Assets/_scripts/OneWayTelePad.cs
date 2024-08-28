using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmadiateTeleportation : MonoBehaviour
{
    public GameObject playerObj;
    public GameObject teleportationDestination;

    private bool isPlayerInTriggerPad = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInTriggerPad)
        {
            if (teleportationDestination != null)
            {
                Debug.Log("Player touched the immediate teleportation pad!");

                playerObj.transform.position = teleportationDestination.transform.position;
            }
        }
        else
        {
            Debug.LogWarning("Teleportation destination is not assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerObj)
        {
            isPlayerInTriggerPad = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerObj)
        {
            isPlayerInTriggerPad = false;
        }
    }
}
