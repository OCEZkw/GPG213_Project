using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeckManager
{
    List<GameObject> GetHand();
    // Add other methods that both DeckManager and TutorialDeckManager should implement
}