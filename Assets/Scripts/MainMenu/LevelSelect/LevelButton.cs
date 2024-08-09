using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int levelIndex;
    private LevelSelectManager levelSelectManager;
    private Vector3 originalScale;
    public float hoverScaleFactor = 1.1f;
    public float tweenDuration = 0.2f;

    private void Start()
    {
        levelSelectManager = FindObjectOfType<LevelSelectManager>();
        GetComponent<Button>().onClick.AddListener(() => levelSelectManager.SelectLevel(levelIndex));
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        levelSelectManager.ShowLevelInfo(levelIndex);
        transform.DOScale(originalScale * hoverScaleFactor, tweenDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        levelSelectManager.HideLevelInfo();
        transform.DOScale(originalScale, tweenDuration);
    }
}
