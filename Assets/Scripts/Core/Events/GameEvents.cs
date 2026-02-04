using System;
using System.Collections.Generic;

public static class GameEvents
{
    // UI
    public static Action<bool> OnInventoryToggle;       // true=open, false=close
    public static Action OnInventoryClosed;
    public static Action OnInventoryChanged;
    public static Action OnEquipmentChanged;

    //public static Action<Inventory, int> OnSlotHovered;            // slotIndex
    //public static Action OnSlotHoverExit;

    //public static Action<Inventory, int> OnContextMenuRequest;     // slotIndex
    public static Action<ItemUIContext> OnContextMenuRequest;
    public static Action OnContextMenuClose;

    public static Action<ItemUIContext> OnTooltipRequest;
    public static Action OnTooltipHide;

    // Inventory logic
    public static Action<int> OnItemUsed;               // slotIndex
    public static Action<int> OnItemDropped;
    public static Action<int> OnItemInspected;

    // Equipment logic
    public static Action<EquipmentData> OnEquipRequested;
    public static Action<EquipmentSlot> OnUnequipRequested;
    public static Action<EquipmentData, List<StatModifier>> OnEquipped;
    public static Action<EquipmentData, List<StatModifier>> OnUnequipped;
    public static Action<EquipmentData> OnEquipmentSlotHovered;
    public static Action<EquipmentData> OnEquipmentContextMenuRequest;

    // World
    public static Action<ItemData, int> OnItemPicked;
}