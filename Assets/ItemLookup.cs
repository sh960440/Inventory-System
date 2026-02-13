public static class ItemLookup
{
    // Find the first ItemData matching the given itemName in the inventory. Returns null if not found.
    public static ItemData FindInInventory(
        Inventory inventory,
        string itemName
    )
    {
        if (inventory == null) return null;

        foreach (var slot in inventory.slots)
        {
            if (slot.item != null && slot.item.itemName == itemName)
                return slot.item;
        }

        return null;
    }
}
