using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingName : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3f, 0);
    public string guardName;

    private TextMeshPro textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();

        if (string.IsNullOrEmpty(guardName))
        {
            guardName = "Guard";
        }

        textMesh.text = guardName;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
