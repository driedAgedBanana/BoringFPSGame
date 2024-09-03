using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLootChest : BaseLootDrop
{
    public float interactionDistance = 3f;  // Maximum distance to interact with the chest

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                // Check if the object hit by the raycast is this chest
                if (hit.collider.gameObject == gameObject)
                {
                    OpenChest();
                }
            }
        }
    }

    public void OpenChest()
    {
        Destroy(gameObject);
        DropLoot();
    }
}
