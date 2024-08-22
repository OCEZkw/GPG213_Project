using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public string leaderTag = "Leader";
    public string guardTag = "Guard";

    void Start()
    {
        IgnoreCollisionsBetweenTags(leaderTag, guardTag);
    }

    void IgnoreCollisionsBetweenTags(string tag1, string tag2)
    {
        GameObject[] objectsWithTag1 = GameObject.FindGameObjectsWithTag(tag1);
        GameObject[] objectsWithTag2 = GameObject.FindGameObjectsWithTag(tag2);

        foreach (GameObject obj1 in objectsWithTag1)
        {
            Collider2D collider1 = obj1.GetComponent<Collider2D>();
            if (collider1 == null) continue;

            foreach (GameObject obj2 in objectsWithTag2)
            {
                Collider2D collider2 = obj2.GetComponent<Collider2D>();
                if (collider2 == null) continue;

                Physics2D.IgnoreCollision(collider1, collider2, true);
            }
        }
    }
}
