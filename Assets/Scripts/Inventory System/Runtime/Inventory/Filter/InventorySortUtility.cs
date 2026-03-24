using System;
using System.Collections.Generic;

/// <summary>
/// Static helpers for inventory slot sorting.
/// </summary>
public static class InventorySortUtility
{
    public static void SortSlots(
        List<InventorySlot> slots,
        InventorySortType sortType,
        SortOrder sortOrder,
        List<InventorySlot> filledBuffer,
        List<InventorySlot> emptyBuffer)
    {
        filledBuffer?.Clear();
        emptyBuffer?.Clear();

        if (filledBuffer == null || emptyBuffer == null)
            return;

        foreach (var s in slots)
        {
            if (s.item == null)
                emptyBuffer.Add(s);
            else
                filledBuffer.Add(s);
        }

        filledBuffer.Sort((a, b) => CompareSlots(a, b, sortType, sortOrder));

        slots.Clear();
        slots.AddRange(filledBuffer);
        slots.AddRange(emptyBuffer);
    }

    static int CompareSlots(InventorySlot a, InventorySlot b, InventorySortType sortType, SortOrder sortOrder)
    {
        if (a.item == null || b.item == null)
            return 0;

        int result = 0;

        switch (sortType)
        {
            case InventorySortType.Name:
                result = string.Compare(a.item.itemName, b.item.itemName);
                break;

            case InventorySortType.Rarity:
                result = a.item.rarity.CompareTo(b.item.rarity);
                break;

            case InventorySortType.Category:
                result = a.item.category.CompareTo(b.item.category);
                break;

            case InventorySortType.Count:
                result = a.count.CompareTo(b.count);
                break;
        }

        if (sortOrder == SortOrder.Descending)
            result = -result;

        return result;
    }
}
