using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A hotbar slot UI that displays the assigned item and supports basic interactions.
/// </summary>
public class HotbarSlotUI : UISlotBase, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Visual")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Sprite defaultBackground;
    [SerializeField] private Sprite equippedBackground;

    [Header("Drag")]
    [SerializeField] private DraggableItemUI dragUI;

    private Hotbar _hotbar;
    private Equipment _equipmentManager;
    private int _index;
    private DragItemContext? _currentDrag;

    private void OnEnable()
    {
        InventoryEvents.OnItemDragBegin += OnExternalDragBegin;
        InventoryEvents.OnItemDragEnd += OnExternalDragEnd;
        InventoryEvents.EquipmentChanged += Refresh;
    }

    private void OnDisable()
    {
        InventoryEvents.OnItemDragBegin -= OnExternalDragBegin;
        InventoryEvents.OnItemDragEnd -= OnExternalDragEnd;
        InventoryEvents.EquipmentChanged -= Refresh;
    }

    /// <summary>
    /// Wired by HotbarUIController after instantiation.
    /// </summary>
    public void Setup(Hotbar hotbar, Equipment equipmentManager, int slotIndex, DraggableItemUI ui)
    {
        _hotbar = hotbar;
        _equipmentManager = equipmentManager;
        _index = slotIndex;
        keyText.text = (slotIndex + 1).ToString();
        dragUI = ui;
    }

    public void Refresh()
    {
        var invSlot = _hotbar.GetInventorySlot(_index);

        if (invSlot == null || invSlot.Item == null)
        {
            icon.enabled = false;
            countText?.gameObject.SetActive(false);
            backgroundImage.sprite = defaultBackground;
            return;
        }

        icon.sprite = invSlot.Item.Icon;
        icon.enabled = true;

        RefreshCount(invSlot);
        RefreshEquippedState(invSlot.Item, _hotbar.GetBoundInventorySlotIndex(_index));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dragUI == null)
            return;

        var invSlot = _hotbar.GetInventorySlot(_index);
        if (invSlot == null)
            return;

        var ctx = new DragItemContext(null, inventorySlotIndex: -1, hotbarIndex: _index, item: invSlot.Item);

        dragUI.BeginDrag(ctx, invSlot.Item.Icon);
        InventoryEvents.OnItemDragBegin?.Invoke(ctx);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragUI?.FollowMouse();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragUI == null)
            return;

        dragUI.EndDrag();
        InventoryEvents.OnItemDragEnd?.Invoke();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_currentDrag == null)
            return;

        var ctx = _currentDrag.Value;

        // From Inventory to Hotbar
        if (ctx.Inventory != null)
        {
            _hotbar.Assign(_index, ctx.Inventory, ctx.InventorySlotIndex);
            return;
        }

        // From Hotbar to Hotbar
        if (ctx.HotbarIndex >= 0 && ctx.HotbarIndex != _index)
        {
            _hotbar.Swap(_index, ctx.HotbarIndex);
        }
    }

    public void ClearSelf()
    {
        _hotbar.Clear(_index);
    }

    protected override void OnDoubleClick()
    {
        if (_hotbar == null)
            return;
        if (!_hotbar.AllowDoubleClickUse)
            return;
        if (!_hotbar.ValidHotbarIndex(_index))
            return;

        var invSlot = _hotbar.GetInventorySlot(_index);
        if (invSlot == null || invSlot.Item == null)
            return;

        InventoryEvents.HotbarUseRequested?.Invoke(invSlot);
    }

    private void OnExternalDragBegin(DragItemContext ctx)
    {
        _currentDrag = ctx;
    }

    private void OnExternalDragEnd()
    {
        _currentDrag = null;
    }

    private void RefreshCount(InventorySlot invSlot)
    {
        if (countText == null)
            return;

        if (invSlot.Item is EquipmentData || invSlot.Count <= 1)
        {
            countText.gameObject.SetActive(false);
            return;
        }

        countText.text = invSlot.Count.ToString();
        countText.gameObject.SetActive(true);
    }

    private void RefreshEquippedState(ItemData item, int boundInventorySlotIndex)
    {
        if (backgroundImage == null)
            return;

        if (item is not EquipmentData eq)
        {
            backgroundImage.sprite = defaultBackground;
            return;
        }

        bool source = _equipmentManager != null
                      && _equipmentManager.IsInventorySlotSourceOfEquippedItem(boundInventorySlotIndex, eq);

        backgroundImage.sprite = source ? equippedBackground : defaultBackground;
    }
}