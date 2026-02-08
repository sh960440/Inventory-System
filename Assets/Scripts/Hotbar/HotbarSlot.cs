[System.Serializable]
public class HotbarSlot
{
    public Inventory inventory;
    public int inventorySlotIndex = -1;

    public bool IsEmpty => inventorySlotIndex < 0;

    public ItemData Item =>
        inventory != null && inventorySlotIndex >= 0
            ? inventory.slots[inventorySlotIndex].item
            : null;

    public int Count =>
        inventory != null && inventorySlotIndex >= 0
            ? inventory.slots[inventorySlotIndex].count
            : 0;

    public void Clear()
    {
        inventorySlotIndex = -1;
    }
}