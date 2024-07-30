using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnInventoryClick : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardLevelText;

    private Button button;
    private IInventoryClickBehavior clickBehavior;
    private Inventory.InventoryItem item;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void SetItem(Inventory.InventoryItem newItem)
    {
        item = newItem;
        UpdateVisuals();
    }

    public void SetClickBehavior(IInventoryClickBehavior behavior)
    {
        clickBehavior = behavior;
    }

    private void UpdateVisuals()
    {
        if (item != null)
        {
            cardImage.sprite = item.cardSprite;
            cardNameText.text = item.cardName;
            cardLevelText.text = $"Lv. {item.level}";
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnClick()
    {
        if (item != null && clickBehavior != null)
        {
            clickBehavior.OnItemClicked(item);
        }
    }
}
