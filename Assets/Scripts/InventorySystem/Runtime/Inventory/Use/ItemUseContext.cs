public readonly struct ItemUseContext
{
    public readonly Inventory Inventory;
    public readonly Equipment Equipment;

    public ItemUseContext(Inventory inventory, Equipment equipment)
    {
        Inventory = inventory;
        Equipment = equipment;
    }
}