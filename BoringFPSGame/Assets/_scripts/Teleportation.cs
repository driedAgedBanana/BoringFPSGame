using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
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
        //playerItSelf.transform.position = teleporationDestination.transform.position;
        //Debug.Log("Player touched the teleport pad!");

        if (other.CompareTag("Player"))
        {

        }
    }
}
