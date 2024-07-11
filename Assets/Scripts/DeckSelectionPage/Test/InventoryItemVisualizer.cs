using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemVisualizer : MonoBehaviour
{
    public void Initialize(Inventory.InventoryItem item)
    {
        gameObject.name = $"{item.cardName}";
    }
}
