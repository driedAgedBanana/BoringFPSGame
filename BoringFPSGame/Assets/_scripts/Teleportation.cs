using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    public GameObject playerObj;
    public GameObject teleporationDestination;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerObj)
        {
            if (teleporationDestination != null)
            {
                Debug.Log("Current Player Position: " + playerObj.transform.position);
                Debug.Log("Teleportation Destination Position: " + teleporationDestination.transform.position);

                playerObj.transform.position = teleporationDestination.transform.position;

                Debug.Log("Player teleported to: " + playerObj.transform.position);
            }
            else
            {
                Debug.LogError("Teleportation destination is not assigned.");
            }
        }
    }
}
