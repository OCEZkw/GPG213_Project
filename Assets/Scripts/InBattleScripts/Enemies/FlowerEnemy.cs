using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerEnemy : MonoBehaviour
{
    public enum FlowerType { Healing, CardLocking }
    public FlowerType flowerType;
    public int healAmount = 50;
    private TripartiteBoss tripartiteBoss;
    private DeckManager deckManager;
    private GameObject lockedCard;

    private void Start()
    {
        tripartiteBoss = FindObjectOfType<TripartiteBoss>();
        deckManager = FindObjectOfType<DeckManager>();
        Debug.Log($"FlowerEnemy of type {flowerType} initialized.");
    }

    public void PerformAction()
    {
        if (flowerType == FlowerType.Healing && tripartiteBoss != null)
        {
            TripartiteBossPart mainBody = tripartiteBoss.mainBody;
            if (mainBody != null)
            {
                mainBody.Heal(healAmount);
                Debug.Log($"Healing Flower performed action: Healed boss main body for {healAmount} HP.");
            }
            else
            {
                Debug.LogWarning("Healing Flower couldn't find the main body of the boss.");
            }
        }
        else if (flowerType == FlowerType.CardLocking && deckManager != null && lockedCard == null)
        {
            lockedCard = deckManager.LockRandomCard();
            if (lockedCard != null)
            {
                Debug.Log($"Card-Locking Flower locked card: {lockedCard.name}");
            }
            else
            {
                Debug.Log("Card-Locking Flower couldn't lock any card.");
            }
        }
        else
        {
            Debug.LogWarning($"FlowerEnemy of type {flowerType} failed to perform action. TripartiteBoss: {tripartiteBoss}, DeckManager: {deckManager}");
        }
    }

    private void OnDestroy()
    {
        if (flowerType == FlowerType.CardLocking && deckManager != null && lockedCard != null)
        {
            deckManager.UnlockCard(lockedCard);
            Debug.Log($"Card-Locking Flower destroyed. Unlocked card: {lockedCard.name}");
        }
    }
}
