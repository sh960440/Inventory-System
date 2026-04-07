using System;

/// <summary>
/// Static helpers for inventory slot filtering (category + name search).
/// </summary>
public static class InventoryFilterUtility
{
    /// <summary>
    /// Returns true if the slot should be shown. Empty _slots are shown only when showEmptySlot is true; otherwise the slot must pass category and search filters.
    /// </summary>
    public static bool PassFilter(
        InventorySlot slot,
        ItemCategory[] categories,
        string searchTrimmed,
        bool showEmptySlot)
    {
        if (slot.Item == null)
            return showEmptySlot;

        if (!PassCategory(slot, categories))
            return false;
        if (!PassSearch(slot, searchTrimmed))
            return false;

        return true;
    }

    /// <summary>
    /// Returns true if no categories are selected, or if the item matches at least one of them.
    /// </summary>
    public static bool PassCategory(InventorySlot slot, ItemCategory[] categories)
    {
        if (categories == null || categories.Length == 0)
            return true;

        if (slot.Item == null)
            return false;

        foreach (var c in categories)
        {
            if (slot.Item.category == c)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Returns true if the search is empty or the item name contains the search text.
    /// </summary>
    public static bool PassSearch(InventorySlot slot, string searchTrimmed)
    {
        if (string.IsNullOrEmpty(searchTrimmed))
            return true;

        if (slot.Item == null)
            return false;

        var itemName = slot.Item.itemName;
        /* var description = slot.item.description; */ // TBD

        return !string.IsNullOrEmpty(itemName) &&
               itemName.IndexOf(searchTrimmed, StringComparison.OrdinalIgnoreCase) >= 0;
        
        /* return 
               (!string.IsNullOrEmpty(itemName) && itemName.IndexOf(searchTrimmed, StringComparison.OrdinalIgnoreCase) >= 0) ||
               (!string.IsNullOrEmpty(description) && description.IndexOf(searchTrimmed, StringComparison.OrdinalIgnoreCase) >= 0); */ // TBD
    }

    /// <summary>
    /// Empty _slots are shown only when no category filter is set and there is no active search text.
    /// </summary>
    public static bool ShouldShowEmptySlot(ItemCategory[] categories, string searchTrimmed)
    {
        bool noCategoryFilter = categories == null || categories.Length == 0;
        bool hasSearch = !string.IsNullOrEmpty(searchTrimmed);
        return noCategoryFilter && !hasSearch;
    }
}