using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;

public class GachaManager : MonoBehaviour
{
    public static GachaManager Instance { get; private set; }

    private Button singleSummonButton;
    private Button multiSummonButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("GachaManager Start method called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAndSetupButtons();
    }

    private void FindAndSetupButtons()
    {
        singleSummonButton = GameObject.Find("SingleSummonButton")?.GetComponent<Button>();
        multiSummonButton = GameObject.Find("MultiSummonButton")?.GetComponent<Button>();

        if (singleSummonButton != null)
        {
            singleSummonButton.onClick.RemoveAllListeners();
            singleSummonButton.onClick.AddListener(SingleSummon);
        }

        if (multiSummonButton != null)
        {
            multiSummonButton.onClick.RemoveAllListeners();
            multiSummonButton.onClick.AddListener(MultiSummon);
        }
    }

    private void SingleSummon()
    {
        if (GachaSystem.Instance != null)
        {
            CardSO summonedCard = GachaSystem.Instance.SummonSingleCard();
            LoadCardSummonScene(new List<CardSO> { summonedCard });
        }
        else
        {
            Debug.LogError("GachaSystem instance is null!");
        }
    }

    private void MultiSummon()
    {
        if (GachaSystem.Instance != null)
        {
            List<CardSO> summonedCards = GachaSystem.Instance.SummonMultipleCards(10);
            LoadCardSummonScene(summonedCards);
        }
        else
        {
            Debug.LogError("GachaSystem instance is null!");
        }
    }

    private void LoadCardSummonScene(List<CardSO> cards)
    {
        CardSummonSceneManager.summonedCards = cards;
        SceneManager.LoadScene("CardSummon");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}