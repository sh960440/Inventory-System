using System;

/// <summary>
/// Static helpers for inventory slot filtering (category + search).
/// </summary>
public static class InventoryFilterUtility
{
    public static bool PassFilter(
        InventorySlot slot,
        ItemCategory[] categories,
        string searchKeywordLower,
        bool showEmptySlot)
    {
        if (slot.item == null)
            return showEmptySlot;

        if (!PassCategory(slot, categories))
            return false;
        if (!PassSearch(slot, searchKeywordLower))
            return false;

        return true;
    }

    public static bool PassCategory(InventorySlot slot, ItemCategory[] categories)
    {
        if (categories == null || categories.Length == 0)
            return true;

        if (slot.item == null)
            return false;

        foreach (var c in categories)
        {
            if (slot.item.category == c)
                return true;
        }

        return false;
    }

    public static bool PassSearch(InventorySlot slot, string searchKeywordLower)
    {
        if (string.IsNullOrEmpty(searchKeywordLower))
            return true;

        if (slot.item == null)
            return false;

        var itemName = slot.item.itemName;
        /* var description = slot.item.description; */ // TBD

        return !string.IsNullOrEmpty(itemName) &&
               itemName.IndexOf(searchKeywordLower, StringComparison.OrdinalIgnoreCase) >= 0;
        
        /* return 
               (!string.IsNullOrEmpty(itemName) && itemName.IndexOf(searchKeywordLower, StringComparison.OrdinalIgnoreCase) >= 0) ||
               (!string.IsNullOrEmpty(description) && description.IndexOf(searchKeywordLower, StringComparison.OrdinalIgnoreCase) >= 0); */ // TBD
    }

    public static bool ShouldShowEmptySlot(ItemCategory[] categories, string searchKeyword)
    {
        bool isAllCategory = categories == null || categories.Length == 0;
        bool hasSearch = !string.IsNullOrEmpty(searchKeyword?.Trim());
        return isAllCategory && !hasSearch;
    }
}
