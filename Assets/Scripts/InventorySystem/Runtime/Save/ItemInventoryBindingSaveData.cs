/// <summary>
/// Represents a saved binding between an item ID and its corresponding inventory slot.
/// </summary>
[System.Serializable]
public class ItemInventoryBindingSaveData
{
    public string itemId;

    /// <summary>
    /// The index of the inventory slot associated with the item, or -1 if none is assigned.
    /// </summary>
    public int inventorySlotIndex = -1;

    /// <summary>
    /// Determines whether the specified item ID correctly corresponds to the item stored in the given inventory slot.
    /// </summary>
    public static bool IsBindingValid(Inventory inventory, IItemDatabase itemDatabase, string itemId, int inventorySlotIndex)
    {
        if (inventory == null || itemDatabase == null || string.IsNullOrEmpty(itemId))
            return false;
        if (inventorySlotIndex < 0 || inventorySlotIndex >= inventory.SlotCount)
            return false;

        var invSlot = inventory.GetSlot(inventorySlotIndex);
        if (invSlot?.Item == null)
            return false;

        var def = itemDatabase.Get(itemId);
        return def != null && invSlot.Item.Id == itemId;
    }
}