using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, IInventoryReadOnly
{
    public List<InventorySlot> slots = new();
    public IReadOnlyList<InventorySlot> Slots => slots;
    public int initialCapacity;
    public ItemCategory[] currentCategories => _filterState.CurrentCategories;
    readonly InventoryFilterState _filterState = new InventoryFilterState();
    private InventorySortType currentSortType = InventorySortType.None;
    private SortOrder currentSortOrder = SortOrder.Ascending;

    public InventorySortType CurrentSortType => currentSortType;
    public SortOrder CurrentSortOrder => currentSortOrder;
    private bool allowStacking = true;
    private bool allowSplitStack = true;
    private Equipment equipmentManager;
    readonly InventoryUseHandlerRegistry useHandlerRegistry = new InventoryUseHandlerRegistry();
    readonly List<InventorySlot> _sortFilled = new List<InventorySlot>();
    readonly List<InventorySlot> _sortEmpty = new List<InventorySlot>();

    public bool IsOpen { get; private set; }
    public bool AllowDoubleClickUse { get; private set; }

    public void SetOpen(bool open)
    {
        if (IsOpen == open)
            return;

        IsOpen = open;

        if (open)
            InventoryEvents.InventoryToggleRequested?.Invoke(true);
        else
            InventoryEvents.InventoryCloseRequested?.Invoke();
    }

    public int SlotCount => slots.Count;

    public bool Valid(int i) => i >= 0 && i < slots.Count;
    
    public InventorySlot GetSlot(int index)
    {
        if (!Valid(index)) return null;
        return slots[index];
    }

    void OnEnable()
    {
        InventoryEvents.ItemUsed += UseSlot;
        InventoryEvents.RemoveItemRequested += RemoveItem;
        InventoryEvents.ItemInspected += InspectItem;
        InventoryEvents.AddItemRequested += OnItemAddedHandler;
        InventoryEvents.HotbarUseRequested += UseSlot;
        InventoryEvents.SplitStackRequested += HandleSplitStack;
    }

    void OnDisable()
    {
        InventoryEvents.ItemUsed -= UseSlot;
        InventoryEvents.RemoveItemRequested -= RemoveItem;
        InventoryEvents.ItemInspected -= InspectItem;
        InventoryEvents.AddItemRequested -= OnItemAddedHandler;
        InventoryEvents.HotbarUseRequested -= UseSlot;
        InventoryEvents.SplitStackRequested -= HandleSplitStack;
    }

    public void ApplyConfig(ItemSystemConfiguration config, Equipment em)
    {
        initialCapacity = config.InventoryRows * config.InventoryColumns;

        if (slots.Count == 0)
        {
            for (int i = 0; i < initialCapacity; i++)
                slots.Add(new InventorySlot());
        }

        equipmentManager = em;
        allowStacking = config.AllowStacking;
        allowSplitStack = config.AllowSplitStack;
        AllowDoubleClickUse = config.AllowInventoryDoubleClickUse;

        useHandlerRegistry.EnsureDefaults();
    }

    public void ClearUseHandlers()
    {
        useHandlerRegistry.Clear();
    }

    public void RegisterUseHandler(IItemUseHandler handler)
    {
        useHandlerRegistry.Register(handler);
    }

    public void EnsureUseHandlers()
    {
        useHandlerRegistry.EnsureDefaults();
    }

    void OnItemAddedHandler(ItemData item, int amount)
    {
        if (!AddItem(item, amount))
            Debug.Log("Inventory full");
    }

    public bool AddItem(ItemData item, int amount)
    {
        // Try stacking into existing slots
        if (item.stackable && allowStacking)
        {
            foreach (var slot in slots)
            {
                if (slot.item == item && slot.count < item.maxStack)
                {
                    int space = item.maxStack - slot.count;
                    int add = Mathf.Min(space, amount);
                    slot.count += add;
                    amount -= add;
                    if (amount <= 0)
                    {
                        InventoryEvents.InventoryChanged?.Invoke();
                        return true;
                    }
                }
            }
        }

        // Find an empty slot
        foreach (var slot in slots)
        {
            if (slot.item == null && amount > 0)
            {
                int add;

                if (allowStacking && item.stackable)
                    add = Mathf.Min(item.maxStack, amount);
                else
                    add = 1;

                slot.item = item;
                slot.count = add;
                amount -= add;
            }
        }

        InventoryEvents.InventoryChanged?.Invoke();

        return amount <= 0; // True: Item(s) added successfully. False: Inventory is full.
    }

    public bool AddItem(ItemData item)
    {
        return AddItem(item, 1);
    }

    public bool CanAddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return true;

        int remaining = amount;

        if (item.stackable && allowStacking)
        {
            foreach (var slot in slots)
            {
                if (slot.item == item && slot.count < item.maxStack)
                {
                    int space = item.maxStack - slot.count;
                    remaining -= Mathf.Min(space, remaining);
                    if (remaining <= 0)
                        return true;
                }
            }
        }

        foreach (var slot in slots)
        {
            if (slot.item == null && remaining > 0)
            {
                int add = allowStacking && item.stackable
                    ? Mathf.Min(item.maxStack, remaining)
                    : 1;

                remaining -= add;
                if (remaining <= 0)
                    return true;
            }
        }

        return remaining <= 0;
    }

    public void RemoveItem(InventorySlot slot, int amount)
    {
        if (slot.item == null) return;
        if (amount <= 0) return;

        slot.count -= amount;

        if (slot.count <= 0)
        {
            slot.item = null;
            slot.count = 0;
        }

        InventoryEvents.InventoryChanged?.Invoke();
    }

    public void RemoveItem(InventorySlot slot)
    {
        RemoveItem(slot, 1);
    }

    public void RemoveItem(int index, int amount)
    {
        if (!Valid(index)) return;
        RemoveItem(slots[index], amount);
    }

    public void RemoveItem(int index)
    {
        RemoveItem(index, 1);
    }

    /// <summary>
    /// Tries to swap or stack two slots. Respects maxStack when stacking.
    /// </summary>
    /// <returns>True if the operation succeeded.</returns>
    public bool TrySwapOrStack(int fromIndex, int toIndex)
    {
        if (!Valid(fromIndex) || !Valid(toIndex) || fromIndex == toIndex)
            return false;

        var a = slots[fromIndex];
        var b = slots[toIndex];

        bool exchangedCells = false;

        if (a.item != null && b.item != null && a.item == b.item && a.item.stackable && allowStacking)
        {
            int space = a.item.maxStack - b.count;
            if (space <= 0)
            {
                SwapSlots(a, b);
                exchangedCells = true;
            }
            else
            {
                int move = Math.Min(space, a.count);
                b.count += move;
                a.count -= move;
                if (a.count <= 0)
                {
                    a.item = null;
                    a.count = 0;
                }
            }
        }
        else
        {
            SwapSlots(a, b);
            exchangedCells = true;
        }

        if (exchangedCells)
            InventoryEvents.InventorySlotsSwapped?.Invoke(fromIndex, toIndex);

        InventoryEvents.InventoryChanged?.Invoke();
        return true;
    }

    static void SwapSlots(InventorySlot a, InventorySlot b)
    {
        var tempItem = a.item;
        var tempCount = a.count;
        a.item = b.item;
        a.count = b.count;
        b.item = tempItem;
        b.count = tempCount;
    }

    void UseSlot(int index)
    {
        if (!Valid(index)) return;
        UseSlot(slots[index], index);
    }

    void UseSlot(InventorySlot slot)
    {
        int index = IndexOfSlot(slot);
        UseSlot(slot, index);
    }

    void UseSlot(InventorySlot slot, int slotIndex)
    {
        if (slot == null || slot.item == null)
            return;

        var ctx = new ItemUseContext(this, equipmentManager, slotIndex);

        if (useHandlerRegistry.TryUse(ctx, slot))
            return;

        Debug.Log($"Used item (no handler): {slot.item.itemName}");
    }

    public int IndexOfSlot(InventorySlot slot)
    {
        if (slot == null) return -1;
        for (int i = 0; i < slots.Count; i++)
        {
            if (ReferenceEquals(slots[i], slot))
                return i;
        }

        return -1;
    }

    void InspectItem(int index)
    {
        if (!Valid(index)) return;
        var item = slots[index].item;
        if (item == null) return;
        Debug.Log($"{item.itemName}\n{item.description}");
    }

    /// <summary>
    /// Fills the given list with slot indices that pass the current filter.
    /// Reuses the list to avoid GC allocation.
    /// </summary>
    public void GetFilteredSlotIndices(List<int> result)
    {
        if (result == null) return;
        _filterState.GetFilteredSlotIndices(slots, result);
    }

    public void SetCategoryFilter(ItemCategory[] categories)
    {
        _filterState.SetCategoryFilter(categories);
        InventoryEvents.InventoryChanged?.Invoke();
    }

    public void SetSearchKeyword(string keyword)
    {
        _filterState.SetSearchKeyword(keyword);
        InventoryEvents.InventoryChanged?.Invoke();
    }

    public bool ShouldShowEmptySlot()
    {
        return _filterState.ShouldShowEmptySlot();
    }

    public bool PassFilter(InventorySlot slot)
    {
        return _filterState.PassFilter(slot);
    }

    public void SetSort(InventorySortType type, SortOrder order)
    {
        currentSortType = type;
        currentSortOrder = order;
        ApplySort();
        InventoryEvents.InventoryChanged?.Invoke();
    }

    /// <summary>
    /// Toggles between ascending and descending, then re-sorts.
    /// </summary>
    public void ToggleSortOrder()
    {
        currentSortOrder = currentSortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        ApplySort();
        InventoryEvents.InventoryChanged?.Invoke();
    }

    void ApplySort()
    {
        InventorySortUtility.SortSlots(slots, currentSortType, currentSortOrder, _sortFilled, _sortEmpty);
    }

    void HandleSplitStack(int index)
    {
        if (!Valid(index)) return;
        if (!allowSplitStack) return;

        var slot = slots[index];
        if (slot.item == null) return;
        if (!slot.item.stackable) return;
        if (slot.count < 2) return;

        int half = slot.count / 2;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = slot.item;
                slots[i].count = half;

                slot.count -= half;

                InventoryEvents.InventoryChanged?.Invoke();
                return;
            }
        }
    }

    public InventorySaveData ToSaveData()
    {
        return InventorySaveDataMapper.ToSaveData(slots);
    }

    public void LoadFromSaveData(InventorySaveData data)
    {
        LoadFromSaveData(data, ItemDatabase.Instance);
    }
    public void LoadFromSaveData(InventorySaveData data, IItemDatabase itemDatabase)
    {
        InventorySaveDataMapper.LoadFromSaveData(data, slots, itemDatabase);
        InventoryEvents.InventoryChanged?.Invoke();
    }
}