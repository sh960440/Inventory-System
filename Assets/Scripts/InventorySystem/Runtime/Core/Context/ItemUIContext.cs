/// <summary>
/// Item and slot info for context menus and tooltips.
/// StackCount is -1 when stack count should not be shown.
/// </summary>
public readonly struct ItemUIContext
{
    public ItemData Item { get; }

    public bool IsFromInventory { get; }

    public bool IsEquipped { get; }

    public int SlotIndex { get; }

    public int StackCount { get; }

    public ItemUIContext(ItemData item, bool isFromInventory, bool isEquipped, int slotIndex, int stackCount)
    {
        Item = item;
        IsFromInventory = isFromInventory;
        IsEquipped = isEquipped;
        SlotIndex = slotIndex;
        StackCount = stackCount;
    }
}