using System;
using System.Collections.Generic;

public static class InventoryEvents
{
    // =========================
    // Inventory State
    // =========================
    public static Action<bool> InventoryToggled;
    public static Action InventoryClosed;
    public static Action InventoryChanged;
    
    // =========================
    // UI Interaction
    // =========================
    public static Action<ItemUIContext> ContextMenuRequested;
    public static Action ContextMenuClosed;
    public static Action<ItemUIContext> TooltipRequested;
    public static Action TooltipHidden;

    // =========================
    // Item Interaction
    // =========================
    public static Action<ItemData, int> ItemAdded;
    public static Action<int, int> ItemRemoved;
    public static Action<int> ItemUsed;
    public static Action<int> ItemInspected;
    public static Action<ConsumableData> ItemConsumed;
    public static Action<ItemData, int> ItemDropped;

    // =========================
    // Equipment
    // =========================
    public static Action<EquipmentData> EquipRequested;
    public static Action<EquipmentSlot> UnequipRequested;
    public static Action EquipmentChanged;

    // =========================
    // Hotbar
    // =========================
    public static Action<InventorySlot> HotbarUseRequested;
    public static Action HotbarChanged;




    // =========================
    // To be sorted
    // =========================
    public static Action<DragItemContext> OnItemDragBegin;
    public static Action OnItemDragEnd;
    
    public static Action<EquipmentData, List<StatModifier>> OnEquipped;
    public static Action<EquipmentData, List<StatModifier>> OnUnequipped;
    public static Action<EquipmentData> OnEquipmentSlotHovered;
    public static Action<EquipmentData> OnEquipmentContextMenuRequest;

    public static Action<int> SplitStackRequested;
}