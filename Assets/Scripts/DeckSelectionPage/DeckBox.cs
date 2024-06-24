using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckBox : MonoBehaviour, IPointerClickHandler
{
    public int boxIndex;
    private DeckSelectionManager deckSelectionManager;

    void Start()
    {
        deckSelectionManager = FindObjectOfType<DeckSelectionManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            deckSelectionManager.OnDeckBoxClick(boxIndex);
        }
    }
}
