public struct ItemUIContext
{
    public ItemData item;
    public bool isFromInventory;
    public bool isEquipped;
    public int slotIndex;
    public int count; // Non-stackable = -1
}
