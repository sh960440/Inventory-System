using System.Collections.Generic;

/// <summary>
/// Holds category and search state for inventory filtering; delegates rules to InventoryFilterUtility.
/// </summary>
public sealed class InventoryFilterState
{
    public ItemCategory[] CurrentCategories { get; private set; }

    private string _searchRaw = "";
    private string _searchTrimmed = "";

    /// <summary>
    /// Replaces the active category filter (null or empty means no category restriction).
    /// </summary>
    public void SetCategoryFilter(ItemCategory[] categories)
    {
        CurrentCategories = categories;
    }

    /// <summary>
    /// Sets the search text; matching is case-insensitive on item name.
    /// </summary>
    public void SetSearchKeyword(string keyword)
    {
        _searchRaw = keyword ?? "";
        _searchTrimmed = _searchRaw.Trim();
    }

    /// <summary>
    /// True when empty inventory cells should stay visible under the current filter.
    /// </summary>
    public bool ShouldShowEmptySlot()
    {
        return InventoryFilterUtility.ShouldShowEmptySlot(CurrentCategories, _searchTrimmed);
    }

    /// <summary>
    /// True if the slot passes category and search filters.
    /// </summary>
    public bool PassFilter(InventorySlot slot)
    {
        if (slot == null)
            return false;

        var showEmpty = ShouldShowEmptySlot();
        return InventoryFilterUtility.PassFilter(
            slot,
            CurrentCategories,
            _searchTrimmed,
            showEmpty);
    }

    /// <summary>
    /// Collects the indices of _slots that match the current filter.
    /// </summary>
    public void GetFilteredSlotIndices(List<InventorySlot> slots, List<int> result)
    {
        if (result == null)
            return;

        result.Clear();
        if (slots == null)
            return;

        var showEmpty = ShouldShowEmptySlot();
        for (int i = 0; i < slots.Count; i++)
        {
            if (InventoryFilterUtility.PassFilter(
                    slots[i],
                    CurrentCategories,
                    _searchTrimmed,
                    showEmpty))
            {
                result.Add(i);
            }
        }
    }
}