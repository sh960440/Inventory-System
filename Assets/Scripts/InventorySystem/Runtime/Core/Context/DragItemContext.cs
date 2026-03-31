/// <summary>
/// Drag items from the bag or hotbar. 
/// From inventory: Inventory and InventorySlotIndex are set, HotbarIndex is -1.
/// From hotbar: Inventory is null, InventorySlotIndex is -1, and HotbarIndex is the source slot.
/// </summary>
public readonly struct DragItemContext
{
    public Inventory Inventory { get; }

    public int InventorySlotIndex { get; }

    public int HotbarIndex { get; }

    public ItemData Item { get; }

    public DragItemContext(Inventory inventory, int inventorySlotIndex, int hotbarIndex, ItemData item)
    {
        Inventory = inventory;
        InventorySlotIndex = inventorySlotIndex;
        HotbarIndex = hotbarIndex;
        Item = item;
    }
}