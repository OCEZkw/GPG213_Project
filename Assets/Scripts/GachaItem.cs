using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GachaItem
{
    public string itemName;
    public int rarity; // 1-Common, 2-Rare, 3-Epic, 4-Legendary
    public Sprite itemImage;
}

public class GachaPool : MonoBehaviour
{
    public GachaItem[] items;
}
