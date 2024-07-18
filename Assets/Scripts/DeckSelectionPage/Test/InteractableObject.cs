using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    LoadScene,
    OpenPanel
}

public class InteractableObject : MonoBehaviour
{
    public InteractionType interactionType;
    public string sceneToLoad;
    public GameObject panelToOpen;
}
