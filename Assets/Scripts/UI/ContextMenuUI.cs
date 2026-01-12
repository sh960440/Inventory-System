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
        useButton.onClick.AddListener(() => { if (boundInventory != null) GameEvents.OnItemUsed?.Invoke(boundSlotIndex); Hide(); });
        dropButton.onClick.AddListener(() => { if (boundInventory != null) boundInventory.DropItem(boundSlotIndex); Hide(); });
        inspectButton.onClick.AddListener(() => { if (boundInventory != null) boundInventory.InspectItem(boundSlotIndex); /* Keep open? close */ Hide(); });

        GameEvents.OnContextMenuRequest += Show;
        GameEvents.OnInventoryClosed += Hide;
    }

    void OnDestroy()
    {
        GameEvents.OnContextMenuRequest -= Show;
        GameEvents.OnInventoryClosed -= Hide;
    }

    public void Show(Inventory inventory, int slotIndex)
    {
        boundInventory = inventory;
        boundSlotIndex = slotIndex;

        rectTransform.position = Input.mousePosition;
        rectTransform.localScale = Vector3.one * 0.8f;

        canvasGroup.alpha = 0;
        LeanTween.scale(rectTransform, Vector3.one, 0.15f);
        LeanTween.alphaCanvas(canvasGroup, 1, 0.15f);
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        boundInventory = null;
        boundSlotIndex = -1;
    }
}