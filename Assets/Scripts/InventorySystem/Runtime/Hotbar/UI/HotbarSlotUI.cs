using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotbarSlotUI : UISlotBase, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image backgroundImage;
    public Image icon;
    public TMP_Text keyText;
    public TMP_Text countText;
    public Sprite defaultBackground;
    public Sprite equippedBackground;
    public DraggableItemUI dragUI;

    Hotbar hotbar;
    Equipment equipmentManager;
    int index;

    DragItemContext? currentDrag;

    void OnEnable()
    {
        InventoryEvents.OnItemDragBegin += OnExternalDragBegin;
        InventoryEvents.OnItemDragEnd += OnExternalDragEnd;
        InventoryEvents.EquipmentChanged += Refresh;
    }

    void OnDisable()
    {
        InventoryEvents.OnItemDragBegin -= OnExternalDragBegin;
        InventoryEvents.OnItemDragEnd -= OnExternalDragEnd;
        InventoryEvents.EquipmentChanged -= Refresh;
    }

    public void SetDragUI(DraggableItemUI ui)
    {
        dragUI = ui;
    }

    void OnExternalDragBegin(DragItemContext ctx)
    {
        currentDrag = ctx;
    }

    void OnExternalDragEnd()
    {
        currentDrag = null;
    }

    public void Setup(Hotbar hb, Equipment em, int slotIndex)
    {
        hotbar = hb;
        equipmentManager = em;
        index = slotIndex;
        keyText.text = (slotIndex + 1).ToString(); // Display key number
    }

    public void Refresh()
    {
        var invSlot = hotbar.GetInventorySlot(index);

        if (invSlot == null || invSlot.item == null)
        {
            icon.enabled = false;
            countText?.gameObject.SetActive(false);
            backgroundImage.sprite = defaultBackground;
            return;
        }

        icon.sprite = invSlot.item.icon;
        icon.enabled = true;

        RefreshCount(invSlot);
        RefreshEquippedState(invSlot.item, hotbar.ValidHotbarIndex(index) ? hotbar.slots[index].boundInventorySlotIndex : -1);
    }

    void RefreshCount(InventorySlot invSlot)
    {
        if (countText == null) return;

        // Don't show count for equipment
        if (invSlot.item is EquipmentData || invSlot.count <= 1)
        {
            countText.gameObject.SetActive(false);
            return;
        }

        countText.text = invSlot.count.ToString();
        countText.gameObject.SetActive(true);
    }

    void RefreshEquippedState(ItemData item, int boundInventorySlotIndex)
    {
        if (backgroundImage == null) return;

        if (item is not EquipmentData eq)
        {
            backgroundImage.sprite = defaultBackground;
            return;
        }

        bool source = equipmentManager != null
                      && equipmentManager.IsInventorySlotSourceOfEquippedItem(boundInventorySlotIndex, eq);

        backgroundImage.sprite = source ? equippedBackground : defaultBackground;
    }

    // =========================
    // Drag FROM Hotbar
    // =========================
    public void OnBeginDrag(PointerEventData eventData)
    {
        var invSlot = hotbar.GetInventorySlot(index);
        if (invSlot == null) return;

        var ctx = new DragItemContext(null, inventorySlotIndex: -1, hotbarIndex: index, item: invSlot.item);

        dragUI.BeginDrag(ctx, invSlot.item.icon);
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

    // =========================
    // Drop ON Hotbar
    // =========================
    public void OnDrop(PointerEventData eventData)
    {
        if (currentDrag == null) return;
        var ctx = currentDrag.Value;

        // Inventory -> Hotbar
        if (ctx.Inventory != null)
        {
            hotbar.Assign(index, ctx.Inventory, ctx.InventorySlotIndex);
            return;
        }

        // Hotbar -> Hotbar swap
        if (ctx.HotbarIndex >= 0 && ctx.HotbarIndex != index)
        {
            hotbar.Swap(index, ctx.HotbarIndex);
        }
    }


    protected override void OnRightClick(PointerEventData eventData)
    {
        Debug.Log($"Hotbar slot {index} right clicked");
    }

    protected override void OnDoubleClick()
    {
        if (hotbar == null) return;
        if (!hotbar.AllowDoubleClickUse) return;
        if (!hotbar.ValidHotbarIndex(index)) return;

        var invSlot = hotbar.GetInventorySlot(index);
        if (invSlot == null || invSlot.item == null) return;

        InventoryEvents.HotbarUseRequested?.Invoke(invSlot);
    }

    public void ClearSelf()
    {
        hotbar.Clear(index);
    }
}