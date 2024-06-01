using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmHandler : MonoBehaviour
{
    public Transform confirmedCardPosition;  // Position where the confirmed card will be displayed
    public Button confirmButton;  // Reference to the confirm button
    public DeckManager deckManager;  // Reference to the DeckManager

    void Start()
    {
        confirmButton.onClick.AddListener(ConfirmCard);
        confirmButton.gameObject.SetActive(false);
    }

    void ConfirmCard()
    {
        if (CardClickHandler.selectedCard != null)
        {
            // Move the selected card to the confirmed position
            CardClickHandler.selectedCard.transform.position = confirmedCardPosition.position;

            // Disable the confirm button and hide the other cards
            confirmButton.gameObject.SetActive(false);
            HideOtherCards();

            // Optionally, disable further interaction with the selected card
            CardClickHandler.selectedCard.GetComponent<Collider2D>().enabled = false;

            // Start the coroutine to handle the confirmed card usage
            StartCoroutine(UseConfirmedCard(CardClickHandler.selectedCard));
        }
    }

    IEnumerator UseConfirmedCard(GameObject confirmedCard)
    {
        yield return new WaitForSeconds(2f);

        // Logic to use the confirmed card (e.g., apply effects, deal damage, etc.)
        Debug.Log("Confirmed card used: " + confirmedCard.name);

        // Replace the used card with a new card from the deck
        ReplaceCardInHand(confirmedCard);
    }

    void HideOtherCards()
    {
        GameObject[] allCards = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in allCards)
        {
            if (card != CardClickHandler.selectedCard)
            {
                card.SetActive(false);
            }
        }
    }

    void ReplaceCardInHand(GameObject usedCard)
    {
        int cardIndex = deckManager.hand.IndexOf(usedCard);
        if (cardIndex != -1)
        {
            deckManager.hand.RemoveAt(cardIndex);
            Destroy(usedCard);

            if (deckManager.deck.Count > 0)
            {
                int randomIndex = Random.Range(0, deckManager.deck.Count);
                GameObject newCard = Instantiate(deckManager.deck[randomIndex], deckManager.handPositions[cardIndex].position, Quaternion.identity);
                var cardClickHandler = newCard.GetComponent<CardClickHandler>();
                cardClickHandler.confirmButton = confirmButton;  // Assign the confirm button
                deckManager.hand.Insert(cardIndex, newCard);
                deckManager.deck.RemoveAt(randomIndex);

                // Show all cards again
                foreach (GameObject card in deckManager.hand)
                {
                    card.SetActive(true);
                }
            }
        }
    }
}
