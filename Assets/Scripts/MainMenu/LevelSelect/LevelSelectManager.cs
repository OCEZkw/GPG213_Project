using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    public string deckSelectSceneName = "DeckSelect";
    public LevelInfo[] levelInfos; // Array of level information

    // UI Elements
    public GameObject infoPanel;
    public GameObject lootPanel;
    public TextMeshProUGUI bossNameText;
    public TextMeshProUGUI bossDescriptionText;
    public TextMeshProUGUI bossDamageTypeText;
    public TextMeshProUGUI bossElementText;
    public Image[] lootSlots; // Array of Image components for loot slots

    private void Start()
    {
        // Hide the info and loot panels at start
        infoPanel.SetActive(false);
        lootPanel.SetActive(false);
    }

    // Call this method when a level button is clicked
    public void SelectLevel(int levelIndex)
    {
        // Store the selected level index in PlayerPrefs
        PlayerPrefs.SetInt("SelectedLevel", levelIndex);
        PlayerPrefs.Save();

        // Load the deck select scene
        SceneManager.LoadScene(deckSelectSceneName);
    }

    public void ShowLevelInfo(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelInfos.Length)
        {
            Debug.LogError("Invalid level index: " + levelIndex);
            return;
        }

        LevelInfo info = levelInfos[levelIndex];

        // Update info panel
        bossNameText.text = "" + info.bossName;
        bossDescriptionText.text = "" + info.bossDescription;
        bossDamageTypeText.text = "" + info.bossDamageType;
        bossElementText.text = "" + info.bossElement;

        // Update loot panel
        UpdateLootSlots(info.lootItems);

        // Show the info and loot panels
        infoPanel.SetActive(true);
        lootPanel.SetActive(true);
    }

    private void UpdateLootSlots(LootItem[] lootItems)
    {
        for (int i = 0; i < lootSlots.Length; i++)
        {
            Image emptyImage = lootSlots[i].transform.GetChild(0).GetComponent<Image>();
            Image filledImage = lootSlots[i].transform.GetChild(1).GetComponent<Image>();

            if (i < lootItems.Length && lootItems[i] != null)
            {
                // Slot has an item
               // emptyImage.gameObject.SetActive(false);
                filledImage.gameObject.SetActive(true);
                filledImage.sprite = lootItems[i].icon;
            }
            else
            {
                // Slot is empty
                emptyImage.gameObject.SetActive(true);
                filledImage.gameObject.SetActive(false);
            }
        }
    }

    public void HideLevelInfo()
    {
        // Hide the info and loot panels
        infoPanel.SetActive(false);
        lootPanel.SetActive(false);
    }
}
