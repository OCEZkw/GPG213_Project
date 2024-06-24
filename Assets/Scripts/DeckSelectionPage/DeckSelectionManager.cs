using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelectionManager : MonoBehaviour
{
    public GameObject[] deckBoxes; // Array of the 10 deck boxes
    public GameObject inventoryPage; // The inventory UI page
    public Card[] inventoryCards; // Array of all cards in the inventory
    public Transform inventoryContent; // Parent object to hold the inventory slots
    public GameObject inventorySlotPrefab; // Prefab for the inventory slot

    private int selectedBoxIndex = -1; // To keep track of which deck box is selected

    public void OnDeckBoxClick(int boxIndex)
    {
        selectedBoxIndex = boxIndex; // Store the selected box index
        OpenInventoryPage(); // Open the inventory page
    }

    void OpenInventoryPage()
    {
        PopulateInventoryPage();
        inventoryPage.SetActive(true); // Show the inventory page
    }

    void PopulateInventoryPage()
    {
        // Clear any existing slots
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

        // Populate inventory slots
        foreach (Card card in inventoryCards)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryContent);
            InventoryCard inventoryCard = slot.GetComponent<InventoryCard>();
            inventoryCard.card = card;
            inventoryCard.UpdateCardDisplay();
        }
    }

    // Attach this to each inventory card
    public void OnInventoryCardClick(Card selectedCard)
    {
        if (selectedBoxIndex >= 0)
        {
            // Replace the card in the selected deck box
          //  deckBoxes[selectedBoxIndex].GetComponent<CardDisplay>().SetCard(selectedCard);
            selectedBoxIndex = -1; // Reset the selected box index
            CloseInventoryPage(); // Close the inventory page
        }
    }

    void CloseInventoryPage()
    {
        inventoryPage.SetActive(false); // Hide the inventory page
    }
}
