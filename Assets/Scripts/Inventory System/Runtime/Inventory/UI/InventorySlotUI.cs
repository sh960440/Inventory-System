using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlotUI : UISlotBase, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int slotIndex;
    public Image backgroundImage;
    [Header("Equipped Visual")]
    public Sprite defaultBackground;
    public Sprite equippedBackground;
    public Image iconImage;
    public TMP_Text countText;
    Inventory inventory;
    public ContextMenuUI contextMenuUI;
    public DraggableItemUI dragUI;
    public TooltipUI tooltipUI;
    SlotHoverService hoverService;
    bool isDragging = false;
    private IEquippedItemLookup equippedItemLookup;
    readonly System.Collections.Generic.List<RaycastResult> _raycastResults = new System.Collections.Generic.List<RaycastResult>(8);

    void OnEnable()
    {
        // Refresh when an item is equipped / unequipped
        InventoryEvents.EquipmentChanged += Refresh;
    }

    void OnDisable()
    {
        InventoryEvents.EquipmentChanged -= Refresh;
    }

    //void EquipmentChanged(EquipmentData item, System.Collections.Generic.List<StatModifier> mods)
    //{
    //    // Refresh when an item is equipped / unequipped
    //    Refresh();
    //}

    public void Setup(Inventory inv, IEquippedItemLookup equippedLookup, int idx, SlotHoverService hover = null)
    {
        inventory = inv;
        slotIndex = idx;
        equippedItemLookup = equippedLookup;
        hoverService = hover;
    }

    //public void Refresh()
    //{
    //    var slot = inventory.slots[slotIndex];

    //    if (slot.item == null)
    //    {
    //        iconImage.sprite = null;
    //        iconImage.enabled = false;
    //        countText.text = "";
    //    }
    //    else
    //    {
    //        iconImage.sprite = slot.item.icon;
    //        iconImage.enabled = true;
    //        iconImage.preserveAspect = true;

    //        countText.text = slot.item.stackable && slot.count > 1 
    //            ? slot.count.ToString() 
    //            : "";
    //    }
    //}

    public void Refresh()
    {
        var slot = inventory.slots[slotIndex];

        bool pass = inventory.PassFilter(slot);
        gameObject.SetActive(pass);

        if (slot.item == null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            countText.text = "";

            backgroundImage.sprite = defaultBackground;

            return;
        }

        iconImage.sprite = slot.item.icon;
        iconImage.enabled = true;
        iconImage.preserveAspect = true;

        countText.text = slot.item.stackable && slot.count > 1
            ? slot.count.ToString()
            : "";

        // ---------- NEW: Equipped State ----------
        bool isEquipped = false;

        if (slot.item is EquipmentData eq)
        {
            isEquipped = equippedItemLookup != null && equippedItemLookup.IsEquipped(eq);
        }

        backgroundImage.sprite = isEquipped ? equippedBackground : defaultBackground;
    }

    public void SetItem(Sprite sprite)
    {
        iconImage.sprite = sprite;
        iconImage.enabled = sprite != null;
        iconImage.preserveAspect = true;
    }

    public void Clear()
    {
        iconImage.sprite = null;
        iconImage.enabled = false;
    }

    public void SetVisible(bool visiable)
    {
        gameObject.SetActive(visiable);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Drag only if left button is clicked
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (inventory.slots[slotIndex].item == null)
            return;

        isDragging = true;
        //dragUI.SetSprite(inventory.slots[slotIndex].item.icon);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;
        //dragUI.Hide();

        // Raycast UI to find which slot to drop
        _raycastResults.Clear();
        EventSystem.current.RaycastAll(eventData, _raycastResults);

        foreach (var r in _raycastResults)
        {
            var other = r.gameObject.GetComponent<InventorySlotUI>();
            if (other != null)
            {
                DropOnto(other.slotIndex);
                return;
            }
        }
    }

    void Update()
    {
        //if (isDragging)
            //dragUI.FollowMouse();
    }

    void DropOnto(int targetIndex)
    {
        if (targetIndex == slotIndex) return;
        inventory.TrySwapOrStack(slotIndex, targetIndex);
    }

    protected override void OnDoubleClick()
    {
        if (inventory == null) return;
        if (!inventory.AllowDoubleClickUse) return;
        if (!inventory.Valid(slotIndex)) return;

        var slot = inventory.slots[slotIndex];
        if (slot.item == null) return;

        InventoryEvents.ItemUsed?.Invoke(slotIndex);
    }

    protected override void OnMiddleClick(PointerEventData eventData)
    {
        var slot = inventory.slots[slotIndex];
        if (slot.item == null) return;

        // Shift + right click -> Split Stack
        //if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        //{
        //    SplitStack();
        //    return;
        //}

        // Open Context Menu
        bool isEquipped = false;
        if (slot.item is EquipmentData eq)
            isEquipped = equippedItemLookup != null && equippedItemLookup.IsEquipped(eq);

        InventoryEvents.ContextMenuRequested?.Invoke(new ItemUIContext
        {
            item = slot.item,
            isFromInventory = true,
            isEquipped = isEquipped,
            slotIndex = slotIndex,
            count = slot.item.stackable && slot.count >= 1 ? slot.count : -1
        });
    }

    //void SplitStack()
    //{
    //    var slot = inventory.slots[slotIndex];
    //    if (slot.item == null) return;
    //    if (slot.count < 2) return;

    //    int half = slot.count / 2;

    //    // Find an empty slot
    //    for (int i = 0; i < inventory.slots.Count; i++)
    //    {
    //        if (inventory.slots[i].item == null)
    //        {
    //            // fill in the slot
    //            inventory.slots[i].item = slot.item;
    //            inventory.slots[i].count = half;

    //            // original slot
    //            slot.count -= half;

    //            //inventory.UpdateUI();
    //            InventoryEvents.InventoryChanged?.Invoke();
    //            return;
    //        }
    //    }
    //}

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverService != null)
            hoverService.SetHovered(slotIndex);

        backgroundImage.color = new Color(1f, 1f, 1f, 0.9f);
        transform.localScale = Vector3.one * 1.05f;

        var slot = inventory.slots[slotIndex];
        if (slot.item == null) return;

        bool isEquipped = false;
        if (slot.item is EquipmentData eq)
            isEquipped = equippedItemLookup != null && equippedItemLookup.IsEquipped(eq);

        InventoryEvents.TooltipRequested?.Invoke(new ItemUIContext
        {
            item = slot.item,
            slotIndex = slotIndex,
            isFromInventory = true,
            isEquipped = isEquipped,
            count = slot.item.stackable && slot.count >= 1 ? slot.count : -1
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverService != null && hoverService.CurrentHoveredIndex == slotIndex)
            hoverService.ClearHovered();

        backgroundImage.color = Color.white;
        transform.localScale = Vector3.one;

        InventoryEvents.TooltipHidden?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var slot = inventory.slots[slotIndex];
        if (slot.item == null) return;

        var ctx = new DragItemContext
        {
            inventory = inventory,
            inventorySlotIndex = slotIndex,
            item = slot.item
        };

        dragUI.BeginDrag(ctx, slot.item.icon);
        InventoryEvents.OnItemDragBegin?.Invoke(ctx);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragUI.FollowMouse();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragUI.EndDrag();
        InventoryEvents.OnItemDragEnd?.Invoke();
    }

    //public void DropItem()
    //{
    //    inventory.DropItem(slotIndex, 1);
    //}

}