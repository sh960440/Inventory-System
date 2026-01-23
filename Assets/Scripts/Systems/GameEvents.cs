using System;
using System.Collections.Generic;

public static class GameEvents
{
    // UI
    public static Action<bool> OnInventoryToggle;       // true=open, false=close
    public static Action OnInventoryClosed;
    public static Action OnInventoryChanged;
    public static Action OnEquipmentChanged;

    public static Action<Inventory, int> OnSlotHovered;            // slotIndex
    public static Action OnSlotHoverExit;

    public static Action<Inventory, int, bool> OnContextMenuRequest;     // slotIndex, isEquipped
    public static Action OnContextMenuClose;

    // Inventory logic
    public static Action<int> OnItemUsed;               // slotIndex
    public static Action<int> OnItemDropped;

    // Equipment logic
    public static Action<EquipmentData> OnEquipRequested;
    public static Action<EquipmentSlot> OnUnequipRequested;
    public static Action<EquipmentData, List<StatModifier>> OnEquipped;
    public static Action<EquipmentData, List<StatModifier>> OnUnequipped;

    // World
    public static Action<ItemData, int> OnItemPicked;
}