using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Displays the item action context menu and handles user interactions.
/// </summary>
public class ContextMenuUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;

    [Header("Actions")]
    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button inspectButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;

    private ItemUIContext _context;

    private void OnEnable()
    {
        if (useButton != null)
            useButton.onClick.AddListener(OnUseClicked);
        if (inspectButton != null)
            inspectButton.onClick.AddListener(OnInspectClicked);
        if (dropButton != null)
            dropButton.onClick.AddListener(OnDropClicked);
        if (equipButton != null)
            equipButton.onClick.AddListener(OnEquipClicked);
        if (unequipButton != null)
            unequipButton.onClick.AddListener(OnUnequipClicked);

        InventoryEvents.ContextMenuRequested += Show;
        InventoryEvents.InventoryCloseRequested += Hide;
    }

    private void OnDisable()
    {
        if (useButton != null)
            useButton.onClick.RemoveListener(OnUseClicked);
        if (inspectButton != null)
            inspectButton.onClick.RemoveListener(OnInspectClicked);
        if (dropButton != null)
            dropButton.onClick.RemoveListener(OnDropClicked);
        if (equipButton != null)
            equipButton.onClick.RemoveListener(OnEquipClicked);
        if (unequipButton != null)
            unequipButton.onClick.RemoveListener(OnUnequipClicked);

        InventoryEvents.ContextMenuRequested -= Show;
        InventoryEvents.InventoryCloseRequested -= Hide;
    }

    /// <summary>
    /// Hides the menu and stops blocking raycasts.
    /// </summary>
    public void Hide()
    {
        if (canvasGroup == null)
            return;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void Show(ItemUIContext ctx)
    {
        _context = ctx;

        if (useButton != null)
            useButton.gameObject.SetActive(false);
        if (dropButton != null)
            dropButton.gameObject.SetActive(false);
        if (inspectButton != null)
            inspectButton.gameObject.SetActive(false);
        if (equipButton != null)
            equipButton.gameObject.SetActive(false);
        if (unequipButton != null)
            unequipButton.gameObject.SetActive(false);

        if (ctx.Item == null || canvasGroup == null)
            return;

        if (ctx.IsFromInventory && inspectButton != null)
            inspectButton.gameObject.SetActive(true);

        if (ctx.IsFromInventory && ctx.Item.Consumable && useButton != null)
            useButton.gameObject.SetActive(true);

        if (ctx.IsFromInventory && !ctx.IsEquipped && dropButton != null)
            dropButton.gameObject.SetActive(true);

        if (ctx.Item is EquipmentData)
        {
            if (equipButton != null)
                equipButton.gameObject.SetActive(ctx.IsFromInventory && !ctx.IsEquipped);
            if (unequipButton != null)
                unequipButton.gameObject.SetActive(ctx.IsEquipped);
        }

        if (rectTransform != null && Mouse.current != null)
            rectTransform.position = Mouse.current.position.ReadValue();

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private void OnUseClicked()
    {
        if (_context.IsFromInventory)
            InventoryEvents.ItemUsed?.Invoke(_context.SlotIndex);
        Hide();
    }

    private void OnInspectClicked()
    {
        if (_context.IsFromInventory)
            InventoryEvents.ItemInspected?.Invoke(_context.SlotIndex);
        Hide();
    }

    private void OnDropClicked()
    {
        if (_context.IsFromInventory && !_context.IsEquipped)
        {
            InventoryEvents.RemoveItemRequested?.Invoke(_context.SlotIndex, 1);
            InventoryEvents.ItemDropped?.Invoke(_context.Item, 1);
        }
        Hide();
    }

    private void OnEquipClicked()
    {
        if (_context.Item is EquipmentData eq && _context.IsFromInventory)
            InventoryEvents.EquipRequested?.Invoke(eq, _context.SlotIndex);
        Hide();
    }

    private void OnUnequipClicked()
    {
        if (_context.Item is EquipmentData eq)
            InventoryEvents.UnequipRequested?.Invoke(eq.EquipSlot);
        Hide();
    }
}