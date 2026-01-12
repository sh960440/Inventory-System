using UnityEngine;
using UnityEngine.UI;
using System;

public class ContextMenuUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    public Button useButton;
    public Button dropButton;
    public Button inspectButton;

    private int boundSlotIndex = -1;
    private Inventory boundInventory;

    void Awake()
    {
        // Button binding behavior (using lambda to delay binding slots)
        useButton.onClick.AddListener(() => { if (boundInventory != null) boundInventory.UseItem(boundSlotIndex); Hide(); });
        dropButton.onClick.AddListener(() => { if (boundInventory != null) boundInventory.DropItem(boundSlotIndex); Hide(); });
        inspectButton.onClick.AddListener(() => { if (boundInventory != null) boundInventory.InspectItem(boundSlotIndex); /* Keep open? close */ Hide(); });
    }

    public void ShowAt(Vector2 screenPos, Inventory inventory, int slotIndex)
    {
        boundInventory = inventory;
        boundSlotIndex = slotIndex;

        rectTransform.position = screenPos;
        rectTransform.localScale = Vector3.one * 0.8f;
        canvasGroup.alpha = 0;

        gameObject.SetActive(true);

        LeanTween.scale(rectTransform, Vector3.one, 0.15f);
        LeanTween.alphaCanvas(canvasGroup, 1, 0.15f);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        boundInventory = null;
        boundSlotIndex = -1;
    }
}