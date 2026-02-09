[System.Serializable]
public class HotbarSlot
{
    //public Inventory inventory;
    //public int inventorySlotIndex = -1;

    //public bool IsEmpty => inventorySlotIndex < 0;

    //public ItemData Item =>
    //    inventory != null && inventorySlotIndex >= 0
    //        ? inventory.slots[inventorySlotIndex].item
    //        : null;

    //public int Count =>
    //    inventory != null && inventorySlotIndex >= 0
    //        ? inventory.slots[inventorySlotIndex].count
    //        : 0;

    //public bool IsValid(Inventory inventory)
    //{
    //    if (inventory == null) return false;
    //    if (inventorySlotIndex < 0) return false;
    //    if (inventorySlotIndex >= inventory.SlotCount) return false;

    //    var slot = inventory.GetSlot(inventorySlotIndex);
    //    return slot != null && slot.item != null;
    //}

    //public void Clear()
    //{
    //    inventorySlotIndex = -1;
    //}

    public Inventory inventory;

    // Main reference to the item in the inventory
    public ItemData item;

    public int boundInventorySlotIndex = -1;

    public bool IsEmpty => item == null;

    public void Clear()
    {
        item = null;
        boundInventorySlotIndex = -1;
        inventory = null;
    }
}