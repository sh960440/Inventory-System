/// <summary>
/// Provides shared context data for item use operations.
/// </summary>
public readonly struct ItemUseContext
{
    /// <summary>
    /// Inventory instance that owns the slot.
    /// </summary>
    public readonly Inventory Inventory;

    /// <summary>
    /// Equipment manager for equip/unequip flows.
    /// </summary>
    public readonly Equipment Equipment;

    /// <summary>
    /// Index of the slot used, or -1 if not tied to a grid index.
    /// </summary>
    public readonly int InventorySlotIndex;

    public ItemUseContext(Inventory inventory, Equipment equipment, int inventorySlotIndex = -1)
    {
        Inventory = inventory;
        Equipment = equipment;
        InventorySlotIndex = inventorySlotIndex;
    }
}