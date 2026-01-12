using System;

public static class GameEvents
{
    // UI
    public static Action<bool> OnInventoryToggle;       // true=open, false=close
    public static Action OnInventoryClosed;

    public static Action<int> OnSlotHovered;            // slotIndex
    public static Action OnSlotHoverExit;

    public static Action<Inventory, int> OnContextMenuRequest;     // slotIndex
    public static Action OnContextMenuClose;

    // Inventory logic
    public static Action<int> OnItemUsed;               // slotIndex
    public static Action<int> OnItemDropped;

    // World
    public static Action<ItemData, int> OnItemPicked;
}