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
    public Button equipButton;
    public Button unequipButton;

    private int boundSlotIndex = -1;
    private Inventory boundInventory;

    void Awake()
    {
        // Button binding behavior (using lambda to delay binding slots)
        useButton.onClick.AddListener(() => { if (boundInventory != null) GameEvents.OnItemUsed?.Invoke(boundSlotIndex); Hide(); });
        dropButton.onClick.AddListener(() => { if (boundInventory != null) boundInventory.DropItem(boundSlotIndex); Hide(); });
        inspectButton.onClick.AddListener(() => { if (boundInventory != null) boundInventory.InspectItem(boundSlotIndex); /* Keep open? close */ Hide(); });

        equipButton.onClick.AddListener(() =>
        {
            var item = boundInventory.slots[boundSlotIndex].item as EquipmentData;
            if (item != null)
                GameEvents.OnEquipRequested?.Invoke(item);

            Hide();
        });
        unequipButton.onClick.AddListener(() =>
        {
            var item = boundInventory.slots[boundSlotIndex].item as EquipmentData;
            if (item != null)
                GameEvents.OnUnequipRequested?.Invoke(item.equipSlot);

            Hide();
        });
    }

    void OnEnable()
    {
        GameEvents.OnContextMenuRequest += Show;
        GameEvents.OnInventoryClosed += Hide;
    }

    void OnDisable()
    {
        GameEvents.OnContextMenuRequest -= Show;
        GameEvents.OnInventoryClosed -= Hide;
    }


    public void Show(Inventory inventory, int slotIndex, bool isEquipped)
    {
        if (inventory == null || slotIndex < 0 || slotIndex >= inventory.slots.Count)
            return;
        
        boundInventory = inventory;
        boundSlotIndex = slotIndex;

        var slot = inventory.slots[slotIndex];
        var item = slot.item;

        // Buttons are unenabled by default
        useButton.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);

        // Consumable
        if (item != null && item.consumable)
        {
            useButton.gameObject.SetActive(true);
        }

        // Equipment
        if (item is EquipmentData eq)
        {
            equipButton.gameObject.SetActive(!isEquipped);
            unequipButton.gameObject.SetActive(isEquipped);
        }

        // Set the position to display
        rectTransform.position = Input.mousePosition;
        rectTransform.localScale = Vector3.one * 0.8f;

        // Display the menu
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