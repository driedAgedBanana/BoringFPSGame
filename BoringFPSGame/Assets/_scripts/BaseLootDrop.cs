using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLootDrop : MonoBehaviour
{
    [Header("Loot")]
    public List<LootTableScript> LootTable = new List<LootTableScript>();

    public void DropLoot()
    {
        //create a list to store loot items that has the drop chance
        List<LootTableScript> eligibleItems = new List<LootTableScript>();

        //check in the list for any items to drop
        foreach(LootTableScript lootItems in LootTable)
        {
            //the chance of a loot being dropped is 0 - 100, the system decide
            if(Random.Range(0f, 100f) <= lootItems.dropChance)
            {
                //if drop, add it to the eligible list
                eligibleItems.Add(lootItems);
            }
        }

        //If there's any eligible item, randomly select one to drop
        if (eligibleItems.Count > 0)
        {
            LootTableScript chosenItem = eligibleItems[Random.Range(0, eligibleItems.Count)];
            // Instantiate the chosen item at the position of the current object
            InstansitateLoot(chosenItem.itemPrefab);
        }
    }

    private void InstansitateLoot(GameObject loot)
    {
        if (LootTable != null)
        {
            //Drop the loot at that position with a default rotation
            Instantiate(loot, transform.position, Quaternion.identity);
        }
    }
}
