using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI tipText;

    [Header("Loading Tips")]
    public string[] loadingTips;
    public float tipChangeInterval = 5f;

    private void Start()
    {
        StartCoroutine(ChangeTips());
    }

    public void UpdateProgress(float progress)
    {
        progressText.text = $"Loading: {Mathf.Round(progress * 100)}%";
    }

    private IEnumerator ChangeTips()
    {
        while (true)
        {
            tipText.text = loadingTips[Random.Range(0, loadingTips.Length)];
            yield return new WaitForSeconds(tipChangeInterval);
        }
    }
}
