using System;
using System.Collections.Generic;

public static class InventoryEvents
{
    // =========================
    // Invnetory Panel
    // =========================
    /// <summary>Request to toggle/open inventory panel. Subscribed by InventoryUIController.</summary>
    public static Action<bool> InventoryToggleRequested;
    /// <summary>Request to close inventory panel. Subscribed by InventoryUIController, ContextMenuUI, TooltipUI.</summary>
    public static Action InventoryCloseRequested;

    // =========================
    // Inventory Slots
    // =========================
    /// <summary>Fired when slots change (add/remove/swap/sort). Subscribed by UI controllers for refresh.</summary>
    public static Action InventoryChanged;
    /// <summary>Request to add an item to inventory. Subscribed by Inventory.</summary>
    public static Action<ItemData, int> AddItemRequested;
    /// <summary>Request to remove item from slot. Args: slotIndex, amount. Fired by ContextMenuUI, GlobalDropArea. Subscribed by Inventory.</summary>
    public static Action<int, int> RemoveItemRequested;
    /// <summary>Request to use item at slot index. Fired by slot DoubleClick, ContextMenuUI. Subscribed by Inventory.</summary>
    public static Action<int> ItemUsed;
    /// <summary>Request to inspect item at slot index. Fired by ContextMenuUI. Subscribed by Inventory.</summary>
    public static Action<int> ItemInspected;
    /// <summary>Request to split stack at slot index. Subscribed by Inventory.</summary>
    public static Action<int> SplitStackRequested;
    /// <summary>Fired when item is dropped to world.</summary>
    public static Action<ItemData, int> ItemDropped;
    /// <summary>Fired when a consumable is used. Subscribed by ConsumableSystem.</summary>
    public static Action<ConsumableData> ItemConsumed;

    // =========================
    // Equipment
    // =========================
    /// <summary>Request to equip an item. Fired by ContextMenuUI, EquipmentUseHandler. Subscribed by Equipment.</summary>
    public static Action<EquipmentData> EquipRequested;
    /// <summary>Request to unequip from slot. Fired by ContextMenuUI, EquipmentUseHandler. Subscribed by Equipment.</summary>
    public static Action<EquipmentSlot> UnequipRequested;
    /// <summary>Fired when equipped items change. Subscribed by slot UI for refresh.</summary>
    public static Action EquipmentChanged;
    /// <summary>Fired when an item is equipped.</summary>
    public static Action<EquipmentData, List<StatModifier>> OnEquipped;
    /// <summary>Fired when an item is unequipped.</summary>
    public static Action<EquipmentData, List<StatModifier>> OnUnequipped;

    // =========================
    // Hotbar
    // =========================
    /// <summary>Request to use item from hotbar. Fired by HotbarSlotUI double-click. Subscribed by Inventory.</summary>
    public static Action<InventorySlot> HotbarUseRequested;
    /// <summary>Fired when hotbar slots change. Subscribed by HotbarUIController.</summary>
    public static Action HotbarChanged;

    // =========================
    // UI Overlays (Tooltip, Context Menu)
    // =========================
    /// <summary>Request to show context menu for an item. Fired by slot UI. Subscribed by ContextMenuUI.</summary>
    public static Action<ItemUIContext> ContextMenuRequested;
    /// <summary>Fired when context menu is closed.</summary>
    public static Action ContextMenuClosed;
    /// <summary>Request to show tooltip. Fired on slot pointer enter. Subscribed by TooltipUI.</summary>
    public static Action<ItemUIContext> TooltipRequested;
    /// <summary>Fired when tooltip should hide. Subscribed by TooltipUI.</summary>
    public static Action TooltipHidden;

    // =========================
    // Drag & Drop
    // =========================
    /// <summary>Fired when drag begins. Subscribed by HotbarSlotUI for drop target detection.</summary>
    public static Action<DragItemContext> OnItemDragBegin;
    /// <summary>Fired when drag ends. Subscribed by HotbarSlotUI.</summary>
    public static Action OnItemDragEnd;
}
