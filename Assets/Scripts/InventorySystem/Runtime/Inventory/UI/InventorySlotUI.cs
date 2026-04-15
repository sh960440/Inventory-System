using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// An inventory slot UI that displays the item stack and supports basic interactions.
/// </summary>
public class InventorySlotUI : UISlotBase, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Visual")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text countText;

    [Header("Equipped")]
    [SerializeField] private Sprite defaultBackground;
    [SerializeField] private Sprite equippedBackground;

    [Header("Drag")]
    [SerializeField] private DraggableItemUI dragUI;

    private IInventoryReadOnly _inventory;
    private IEquippedItemLookup _equippedItemLookup;
    private SlotHoverService _hoverService;
    public int _slotIndex;
    private bool _isDragging = false;
    private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>(8);

    /// <summary>
    /// Index of this slot in the inventory.
    /// </summary>
    public int SlotIndex => _slotIndex;

    private void OnEnable()
    {
        InventoryEvents.EquipmentChanged += Refresh;
    }

    private void OnDisable()
    {
        InventoryEvents.EquipmentChanged -= Refresh;
    }

    /// <summary>
    /// Initializes the slot UI with its data sources and shared drag icon.
    /// </summary>
    public void Setup(IInventoryReadOnly inv, IEquippedItemLookup equippedLookup, int index, SlotHoverService hover, DraggableItemUI drag)
    {
        _inventory = inv;
        _equippedItemLookup = equippedLookup;
        _slotIndex = index;
        _hoverService = hover;
        if (drag != null)
            dragUI = drag;
    }

    /// <summary>
    /// Updates icon, count, visibility and equipped state.
    /// </summary>
    public void Refresh()
    {
        var slot = _inventory.Slots[_slotIndex];

        bool pass = _inventory.PassFilter(slot);
        gameObject.SetActive(pass);

        if (slot.Item == null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            countText.text = "";
            backgroundImage.sprite = defaultBackground;

            if (_hoverService != null && _hoverService.CurrentHoveredIndex == _slotIndex)
                InventoryEvents.TooltipHidden?.Invoke();

            return;
        }

        iconImage.sprite = slot.Item.Icon;
        iconImage.enabled = true;
        iconImage.preserveAspect = true;

        countText.text = slot.Item.Stackable && slot.Count > 1
            ? slot.Count.ToString()
            : "";

        bool isEquipped = IsEquippedEquipment(slot);
        backgroundImage.sprite = isEquipped ? equippedBackground : defaultBackground;

        // After swaps/stacks the model updates but the pointer may still be over this cell
        // without a new PointerEnter, so refresh the tooltip from current slot data.
        if (pass && _hoverService != null && _hoverService.CurrentHoveredIndex == _slotIndex)
        {
            InventoryEvents.TooltipRequested?.Invoke(new ItemUIContext(
                slot.Item,
                isFromInventory: true,
                isEquipped: isEquipped,
                slotIndex: _slotIndex,
                stackCount: slot.Item.Stackable && slot.Count >= 1 ? slot.Count : -1));
        }
    }

    /// <summary>
    /// Sets the slot icon directly.
    /// </summary>
    public void SetItem(Sprite sprite)
    {
        iconImage.sprite = sprite;
        iconImage.enabled = sprite != null;
        iconImage.preserveAspect = true;
    }

    /// <summary>
    /// Clears the icon image.
    /// </summary>
    public void Clear()
    {
        iconImage.sprite = null;
        iconImage.enabled = false;
    }

    /// <summary>
    /// Shows or hides this cell.
    /// </summary>
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (_inventory.Slots[_slotIndex].Item == null)
            return;

        _isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging)
            return;
        _isDragging = false;

        _raycastResults.Clear();
        EventSystem.current.RaycastAll(eventData, _raycastResults);

        foreach (var r in _raycastResults)
        {
            var other = r.gameObject.GetComponent<InventorySlotUI>();
            if (other != null)
            {
                DropOnto(other.SlotIndex);
                return;
            }
        }
    }

    void DropOnto(int targetIndex)
    {
        if (targetIndex == _slotIndex)
            return;
        _inventory.TrySwapOrStack(_slotIndex, targetIndex);
    }

    protected override void OnDoubleClick()
    {
        if (_inventory == null)
            return;
        if (!_inventory.AllowDoubleClickUse)
            return;
        if (!_inventory.Valid(_slotIndex))
            return;

        var slot = _inventory.Slots[_slotIndex];
        if (slot.Item == null)
            return;

        InventoryEvents.ItemUsed?.Invoke(_slotIndex);
    }

    protected override void OnMiddleClick(PointerEventData eventData)
    {
        var slot = _inventory.Slots[_slotIndex];
        if (slot.Item == null)
            return;

        bool isEquipped = IsEquippedEquipment(slot);

        InventoryEvents.ContextMenuRequested?.Invoke(new ItemUIContext(
            slot.Item,
            isFromInventory: true,
            isEquipped: isEquipped,
            slotIndex: _slotIndex,
            stackCount: slot.Item.Stackable && slot.Count >= 1 ? slot.Count : -1));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hoverService != null)
            _hoverService.SetHovered(_slotIndex);

        backgroundImage.color = new Color(1f, 1f, 1f, 0.9f);
        transform.localScale = Vector3.one * 1.05f;

        var slot = _inventory.Slots[_slotIndex];
        if (slot.Item == null)
            return;

        bool isEquipped = IsEquippedEquipment(slot);

        InventoryEvents.TooltipRequested?.Invoke(new ItemUIContext(
            slot.Item,
            isFromInventory: true,
            isEquipped: isEquipped,
            slotIndex: _slotIndex,
            stackCount: slot.Item.Stackable && slot.Count >= 1 ? slot.Count : -1));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_hoverService != null && _hoverService.CurrentHoveredIndex == _slotIndex)
            _hoverService.ClearHovered();

        backgroundImage.color = Color.white;
        transform.localScale = Vector3.one;

        InventoryEvents.TooltipHidden?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dragUI == null)
            return;
        if (_inventory is not Inventory invMono)
            return;

        var slot = _inventory.Slots[_slotIndex];
        if (slot.Item == null)
            return;

        var ctx = new DragItemContext(invMono, _slotIndex, hotbarIndex: -1, item: slot.Item);

        dragUI.BeginDrag(ctx, slot.Item.Icon);
        InventoryEvents.OnItemDragBegin?.Invoke(ctx);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragUI == null)
            return;
        dragUI.FollowMouse();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragUI == null)
            return;
        dragUI.EndDrag();
        InventoryEvents.OnItemDragEnd?.Invoke();
    }

    private bool IsEquippedEquipment(InventorySlot slot)
    {
        if (slot.Item is not EquipmentData eq)
            return false;
        return _equippedItemLookup != null
               && _equippedItemLookup.IsInventorySlotSourceOfEquippedItem(_slotIndex, eq);
    }
}