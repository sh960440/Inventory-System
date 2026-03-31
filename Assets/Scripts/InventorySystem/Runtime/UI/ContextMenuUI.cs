using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ContextMenuUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;

    public Button useButton;
    public Button dropButton;
    public Button inspectButton;
    public Button equipButton;
    public Button unequipButton;

    ItemUIContext context;

    void Awake()
    {
        useButton.onClick.AddListener(() =>
        {
            if (context.IsFromInventory)
                InventoryEvents.ItemUsed?.Invoke(context.SlotIndex);
            Hide();
        });

        inspectButton.onClick.AddListener(() =>
        {
            if (context.IsFromInventory)
                InventoryEvents.ItemInspected?.Invoke(context.SlotIndex);
            Hide();
        });

        dropButton.onClick.AddListener(() =>
        {
            if (context.IsFromInventory && !context.IsEquipped)
            {
                InventoryEvents.RemoveItemRequested?.Invoke(context.SlotIndex, 1);
                InventoryEvents.ItemDropped?.Invoke(context.Item, 1);
            }      
            Hide();
        });

        equipButton.onClick.AddListener(() =>
        {
            if (context.Item is EquipmentData eq && context.IsFromInventory)
                InventoryEvents.EquipRequested?.Invoke(eq, context.SlotIndex);
            Hide();
        });

        unequipButton.onClick.AddListener(() =>
        {
            if (context.Item is EquipmentData eq)
                InventoryEvents.UnequipRequested?.Invoke(eq.equipSlot);
            Hide();
        });
    }

    void OnEnable()
    {
        InventoryEvents.ContextMenuRequested += Show;
        InventoryEvents.InventoryCloseRequested += Hide;
    }

    void OnDisable()
    {
        InventoryEvents.ContextMenuRequested -= Show;
        InventoryEvents.InventoryCloseRequested -= Hide;
    }

    void Show(ItemUIContext ctx)
    {
        context = ctx;

        // Reset
        useButton.gameObject.SetActive(false);
        dropButton.gameObject.SetActive(false);
        inspectButton.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);

        if (ctx.Item == null) return;

        // Inventory only
        if (ctx.IsFromInventory)
        {
            inspectButton.gameObject.SetActive(true);

            if (ctx.Item.consumable)
                useButton.gameObject.SetActive(true);

            if (!ctx.IsEquipped)
                dropButton.gameObject.SetActive(true);
        }

        // Equipment logic
        if (ctx.Item is EquipmentData)
        {
            equipButton.gameObject.SetActive(ctx.IsFromInventory && !ctx.IsEquipped);
            unequipButton.gameObject.SetActive(ctx.IsEquipped);
        }

        rectTransform.position = Mouse.current.position.ReadValue();

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}