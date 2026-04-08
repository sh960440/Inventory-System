using System.Collections.Generic;

/// <summary>
/// Converts between runtime inventory slots and their serialized save-data representation.
/// </summary>
public static class InventorySaveDataMapper
{
    /// <summary>
    /// Converts the current inventory slots into their serialized save-data representation, including explicit entries for empty slots.
    /// </summary>
    public static InventorySaveData ToSaveData(List<InventorySlot> slots)
    {
        var data = new InventorySaveData();
        if (slots == null)
            return data;

        foreach (var slot in slots)
        {
            if (slot == null || slot.Item == null)
            {
                data.slots.Add(new InventorySlotSaveData
                {
                    itemId = null,
                    count = 0
                });
                continue;
            }

            data.slots.Add(new InventorySlotSaveData
            {
                itemId = slot.Item.Id,
                count = slot.Count
            });
        }

        return data;
    }

    /// <summary>
    /// Restores inventory slots from serialized save data using the provided item database.
    /// </summary>
    public static void LoadFromSaveData(
        InventorySaveData data,
        List<InventorySlot> targetSlots,
        IItemDatabase itemDatabase)
    {
        targetSlots.Clear();

        if (data == null || data.slots == null)
            return;

        foreach (var s in data.slots)
        {
            if (s == null || string.IsNullOrEmpty(s.itemId))
            {
                targetSlots.Add(new InventorySlot());
                continue;
            }

            var item = itemDatabase?.Get(s.itemId);
            targetSlots.Add(
                item != null
                    ? new InventorySlot(item, s.count)
                    : new InventorySlot());
        }
    }
}