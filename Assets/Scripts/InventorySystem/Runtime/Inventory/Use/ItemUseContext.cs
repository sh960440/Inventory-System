public readonly struct ItemUseContext
{
    public readonly Inventory Inventory;
    public readonly Equipment Equipment;
    public readonly int InventorySlotIndex;

    public ItemUseContext(Inventory inventory, Equipment equipment, int inventorySlotIndex = -1)
    {
        Inventory = inventory;
        Equipment = equipment;
        InventorySlotIndex = inventorySlotIndex;
    }
}