using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancementClickBehavior : MonoBehaviour, IInventoryClickBehavior
{
    private LevelingInventory levelingInventory;

    void Start()
    {
       // levelingInventory = FindObjectOfType<LevelingInventory>();
    }

    public void OnItemClicked(Inventory.InventoryItem item)
    {
      //  if (levelingInventory != null)
        {
      //      levelingInventory.OnInventoryItemSelected(item);=
        }
    }
}
