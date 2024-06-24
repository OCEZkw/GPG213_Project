using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card1 : MonoBehaviour
{
    [SerializeField] private string cardName;
    [SerializeField] private Sprite sprite;
    private InventoryManager inventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GameObject.Find("Canvas").GetComponent<InventoryManager>();
        inventoryManager.AddItem(cardName, sprite);
    }
}
