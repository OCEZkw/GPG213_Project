using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActivated;
    public CardSlot[] cardSlot;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItem(string cardName, Sprite cardSprite, string itemDescription)
    {
        for (int i = 0; i < cardSlot.Length; i++)
        {
            if(cardSlot[i].isFull == false)
            {
                cardSlot[i].AddItem(cardName, cardSprite, itemDescription);
                return;
            }
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < cardSlot.Length; i++)
        {
            cardSlot[i].selectedShader.SetActive(false);
            cardSlot[i].thisItemSelected = false;
        }
    }
}
