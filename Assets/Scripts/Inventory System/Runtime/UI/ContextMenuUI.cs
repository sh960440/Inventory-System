using UnityEngine;
using UnityEngine.UI;

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
            if (context.isFromInventory)
                InventoryEvents.OnItemUsed?.Invoke(context.slotIndex);
            Hide();
        });

        inspectButton.onClick.AddListener(() =>
        {
            if (context.isFromInventory)
                InventoryEvents.OnItemInspected?.Invoke(context.slotIndex);
            Hide();
        });

        dropButton.onClick.AddListener(() =>
        {
            if (context.isFromInventory && !context.isEquipped)
                InventoryEvents.OnItemDropped?.Invoke(context.slotIndex);
            Hide();
        });

        equipButton.onClick.AddListener(() =>
        {
            if (context.item is EquipmentData eq)
                InventoryEvents.OnEquipRequested?.Invoke(eq);
            Hide();
        });

        unequipButton.onClick.AddListener(() =>
        {
            if (context.item is EquipmentData eq)
                InventoryEvents.OnUnequipRequested?.Invoke(eq.equipSlot);
            Hide();
        });
    }

    void OnEnable()
    {
        InventoryEvents.OnContextMenuRequest += Show;
        InventoryEvents.OnInventoryClosed += Hide;
    }

    void OnDisable()
    {
        InventoryEvents.OnContextMenuRequest -= Show;
        InventoryEvents.OnInventoryClosed -= Hide;
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

        if (ctx.item == null) return;

        // Inventory only
        if (ctx.isFromInventory)
        {
            inspectButton.gameObject.SetActive(true);

            if (ctx.item.consumable)
                useButton.gameObject.SetActive(true);

            if (!ctx.isEquipped)
                dropButton.gameObject.SetActive(true);
        }

        // Equipment logic
        if (ctx.item is EquipmentData)
        {
            equipButton.gameObject.SetActive(ctx.isFromInventory && !ctx.isEquipped);
            unequipButton.gameObject.SetActive(ctx.isEquipped);
        }

        rectTransform.position = Input.mousePosition;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}