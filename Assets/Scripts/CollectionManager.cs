using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public List<GachaItem> collectedItems = new List<GachaItem>();

    public void AddItem(GachaItem item)
    {
        collectedItems.Add(item);
        // Update UI or inventory system
    }
}