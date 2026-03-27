using System.Collections.Generic;

/// SRP helper: maps Inventory runtime slots <-> Save DTO without touching Unity/Events.
public static class InventorySaveDataMapper
{
    public static InventorySaveData ToSaveData(List<InventorySlot> slots)
    {
        var data = new InventorySaveData();
        if (slots == null)
            return data;

        foreach (var slot in slots)
        {
            if (slot == null || slot.item == null)
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
                itemId = slot.item.Id,
                count = slot.count
            });
        }

        return data;
    }

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
                    : new InventorySlot()
            );
        }
    }
}