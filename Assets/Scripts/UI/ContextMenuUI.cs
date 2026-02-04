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
                GameEvents.OnItemUsed?.Invoke(context.slotIndex);
            Hide();
        });

        inspectButton.onClick.AddListener(() =>
        {
            if (context.isFromInventory)
                GameEvents.OnItemInspected?.Invoke(context.slotIndex);
            Hide();
        });

        dropButton.onClick.AddListener(() =>
        {
            if (context.isFromInventory && !context.isEquipped)
                GameEvents.OnItemDropped?.Invoke(context.slotIndex);
            Hide();
        });

        equipButton.onClick.AddListener(() =>
        {
            if (context.item is EquipmentData eq)
                GameEvents.OnEquipRequested?.Invoke(eq);
            Hide();
        });

        unequipButton.onClick.AddListener(() =>
        {
            if (context.item is EquipmentData eq)
                GameEvents.OnUnequipRequested?.Invoke(eq.equipSlot);
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