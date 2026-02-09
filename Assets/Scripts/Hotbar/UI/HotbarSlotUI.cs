using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class HotbarSlotUI : UISlotBase, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image icon;
    public TMP_Text keyText;
    public TMP_Text countText;
    public Image equippedIcon;
    public DraggableItemUI dragUI;

    Hotbar hotbar;
    int index;

    DragItemContext? currentDrag;

    void OnEnable()
    {
        GameEvents.OnItemDragBegin += OnExternalDragBegin;
        GameEvents.OnItemDragEnd += OnExternalDragEnd;
        GameEvents.OnEquipmentChanged += Refresh;
    }

    void OnDisable()
    {
        GameEvents.OnItemDragBegin -= OnExternalDragBegin;
        GameEvents.OnItemDragEnd -= OnExternalDragEnd;
        GameEvents.OnEquipmentChanged -= Refresh;
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

    public void Setup(Hotbar hb, int slotIndex)
    {
        hotbar = hb;
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
            equippedIcon?.gameObject.SetActive(false);
            return;
        }

        icon.sprite = invSlot.item.icon;
        icon.enabled = true;

        RefreshCount(invSlot);
        RefreshEquippedState(invSlot.item);
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

    void RefreshEquippedState(ItemData item)
    {
        if (equippedIcon == null) return;

        if (item is not EquipmentData eq)
        {
            equippedIcon.gameObject.SetActive(false);
            return;
        }

        var mgr = FindFirstObjectByType<EquipmentManager>();
        equippedIcon.gameObject.SetActive(mgr != null && mgr.IsEquipped(eq));
    }

    // =========================
    // Drag FROM Hotbar
    // =========================
    public void OnBeginDrag(PointerEventData eventData)
    {
        var invSlot = hotbar.GetInventorySlot(index);
        if (invSlot == null) return;

        var ctx = new DragItemContext
        {
            inventory = null,               // Hotbar source doesn't have an inventory reference
            inventorySlotIndex = -1,
            hotbarIndex = index,
            item = invSlot.item
        };

        dragUI.BeginDrag(ctx, invSlot.item.icon);
        GameEvents.OnItemDragBegin?.Invoke(ctx);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragUI.FollowMouse();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragUI.EndDrag();
        GameEvents.OnItemDragEnd?.Invoke();
    }

    // =========================
    // Drop ON Hotbar
    // =========================
    public void OnDrop(PointerEventData eventData)
    {
        if (currentDrag == null) return;
        var ctx = currentDrag.Value;

        // Inventory -> Hotbar
        if (ctx.inventory != null)
        {
            hotbar.Assign(index, ctx.inventory, ctx.inventorySlotIndex);
            return;
        }

        // Hotbar -> Hotbar swap
        if (ctx.hotbarIndex >= 0 && ctx.hotbarIndex != index)
        {
            hotbar.Swap(index, ctx.hotbarIndex);
        }
    }


    protected override void OnRightClick(PointerEventData eventData)
    {
        Debug.Log($"Hotbar slot {index} right clicked");
    }

    protected override void OnDoubleClick()
    {
        if (!hotbar.ValidHotbarIndex(index)) return;

        var invSlot = hotbar.GetInventorySlot(index);
        if (invSlot == null || invSlot.item == null) return;

        GameEvents.OnHotbarUseRequested?.Invoke(invSlot);
    }

    public void ClearSelf()
    {
        hotbar.Clear(index);
    }
}