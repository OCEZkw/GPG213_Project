using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryClickBehavior
{
    void OnItemClicked(Inventory.InventoryItem item);
}
