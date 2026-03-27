using System.Collections.Generic;

/// <summary>
/// Small SRP helper that owns Inventory filter/search state and delegates evaluation
/// to <see cref="InventoryFilterUtility"/>.
/// </summary>
public sealed class InventoryFilterState
{
    public ItemCategory[] CurrentCategories { get; private set; }

    string currentSearch = "";
    string currentSearchLower = "";

    public void SetCategoryFilter(ItemCategory[] categories)
    {
        CurrentCategories = categories;
    }

    public void SetSearchKeyword(string keyword)
    {
        currentSearch = keyword ?? "";
        currentSearchLower = currentSearch.Trim();
    }

    public bool ShouldShowEmptySlot()
    {
        return InventoryFilterUtility.ShouldShowEmptySlot(CurrentCategories, currentSearch);
    }

    public bool PassFilter(InventorySlot slot)
    {
        if (slot == null) return false;

        var showEmpty = ShouldShowEmptySlot();
        return InventoryFilterUtility.PassFilter(
            slot,
            CurrentCategories,
            currentSearchLower,
            showEmpty
        );
    }

    public void GetFilteredSlotIndices(List<InventorySlot> slots, List<int> result)
    {
        if (result == null) return;

        result.Clear();
        if (slots == null) return;

        var showEmpty = ShouldShowEmptySlot();
        for (int i = 0; i < slots.Count; i++)
        {
            if (InventoryFilterUtility.PassFilter(
                    slots[i],
                    CurrentCategories,
                    currentSearchLower,
                    showEmpty))
            {
                result.Add(i);
            }
        }
    }
}