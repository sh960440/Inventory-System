using System;
using System.Collections.Generic;

/// <summary>
/// Static helpers for inventory slot sorting.
/// </summary>
public static class InventorySortUtility
{
    /// <summary>
    /// Splits _slots into filled vs empty buffers, sorts filled cells, then rebuilds the list.
    /// </summary>
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
            if (s.Item == null)
                emptyBuffer.Add(s);
            else
                filledBuffer.Add(s);
        }

        filledBuffer.Sort((a, b) => CompareSlots(a, b, sortType, sortOrder));

        slots.Clear();
        slots.AddRange(filledBuffer);
        slots.AddRange(emptyBuffer);
    }

    private static int CompareSlots(InventorySlot a, InventorySlot b, InventorySortType sortType, SortOrder sortOrder)
    {
        if (a.Item == null || b.Item == null)
            return 0;

        int result = 0;

        switch (sortType)
        {
            case InventorySortType.Name:
                result = string.Compare(a.Item.itemName, b.Item.itemName);
                break;

            case InventorySortType.Rarity:
                result = a.Item.rarity.CompareTo(b.Item.rarity);
                break;

            case InventorySortType.Category:
                result = a.Item.category.CompareTo(b.Item.category);
                break;

            case InventorySortType.Count:
                result = a.Count.CompareTo(b.Count);
                break;
        }

        if (sortOrder == SortOrder.Descending)
            result = -result;

        return result;
    }
}