using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public int id;
    public string name;
    public Sprite sprite; // Assuming you are using Sprites for card images

    public Card(int id, string name, Sprite sprite)
    {
        this.id = id;
        this.name = name;
        this.sprite = sprite;
    }
}
