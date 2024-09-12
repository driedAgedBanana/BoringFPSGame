using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLootChest : BaseLootDrop
{
    public float interactionDistance = 3f;  // Maximum distance for interacting with the chest

    private void Update()
    {
        // Check if the player presses the "G" key to interact
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Cast a ray from the camera forward to detect interactions
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            // Perform the raycast and check if it hits within the specified interaction distance
            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                // Check if the object hit by the raycast is this chest
                if (hit.collider.gameObject == gameObject)
                {
                    OpenChest();  // Call OpenChest method to destroy chest and drop loot
                }
            }
        }
    }

    // Method to open the chest, destroy it, and drop loot
    public void OpenChest()
    {
        Destroy(gameObject);  // Destroy the chest object
        DropLoot();           // Call the inherited DropLoot method from BaseLootDrop to drop items
    }
}
