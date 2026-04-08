using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Grid-based inventory: stacking, swap/stack moves, category/search filters, sorting, save/load, and item-use routing.
/// </summary>
public class Inventory : MonoBehaviour, IInventoryReadOnly
{
    [Header("Slots")]
    [SerializeField] private List<InventorySlot> _slots = new List<InventorySlot>();

    private int _initialCapacity;

    private readonly InventoryFilterState _filterState = new InventoryFilterState();
    private InventorySortType _currentSortType = InventorySortType.None;
    private SortOrder _currentSortOrder = SortOrder.Ascending;
    private bool _allowStacking = true;
    private bool _allowSplitStack = true;
    private Equipment _equipmentManager;

    private readonly InventoryUseHandlerRegistry _useHandlerRegistry = new InventoryUseHandlerRegistry();
    private readonly List<InventorySlot> _sortFilled = new List<InventorySlot>();
    private readonly List<InventorySlot> _sortEmpty = new List<InventorySlot>();

    public IReadOnlyList<InventorySlot> Slots => _slots;
    public ItemCategory[] CurrentCategories => _filterState.CurrentCategories;
    public InventorySortType CurrentSortType => _currentSortType;
    public SortOrder CurrentSortOrder => _currentSortOrder;
    public bool IsOpen { get; private set; }
    public bool AllowDoubleClickUse { get; private set; }
    public int SlotCount => _slots.Count;

    private void OnEnable()
    {
        InventoryEvents.ItemUsed += UseSlot;
        InventoryEvents.RemoveItemRequested += RemoveItem;
        InventoryEvents.ItemInspected += InspectItem;
        InventoryEvents.AddItemRequested += OnItemAddedHandler;
        InventoryEvents.HotbarUseRequested += UseSlot;
        InventoryEvents.SplitStackRequested += HandleSplitStack;
    }

    private void OnDisable()
    {
        InventoryEvents.ItemUsed -= UseSlot;
        InventoryEvents.RemoveItemRequested -= RemoveItem;
        InventoryEvents.ItemInspected -= InspectItem;
        InventoryEvents.AddItemRequested -= OnItemAddedHandler;
        InventoryEvents.HotbarUseRequested -= UseSlot;
        InventoryEvents.SplitStackRequested -= HandleSplitStack;
    }

    /// <summary>
    /// Applies layout and rules from config and binds equipment for use handlers.
    /// </summary>
    public void ApplyConfig(ItemSystemConfiguration config, Equipment equipment)
    {
        _initialCapacity = config.InventoryRows * config.InventoryColumns;

        if (_slots.Count == 0)
        {
            for (int i = 0; i < _initialCapacity; i++)
                _slots.Add(new InventorySlot());
        }

        _equipmentManager = equipment;
        _allowStacking = config.AllowStacking;
        _allowSplitStack = config.AllowSplitStack;
        AllowDoubleClickUse = config.AllowInventoryDoubleClickUse;

        _useHandlerRegistry.EnsureDefaults();
    }

    /// <summary>
    /// Clears all registered IItemUseHandler instances.
    /// </summary>
    public void ClearUseHandlers()
    {
        _useHandlerRegistry.Clear();
    }

    /// <summary>
    /// Appends a handler; earlier registrations are tried first when an item is used.
    /// </summary>
    public void RegisterUseHandler(IItemUseHandler handler)
    {
        _useHandlerRegistry.Register(handler);
    }

    /// <summary>
    /// If the registry is empty, registers built-in handlers (consumable, equipment).
    /// </summary>
    public void EnsureUseHandlers()
    {
        _useHandlerRegistry.EnsureDefaults();
    }

    /// <summary>
    /// Opens or closes the inventory UI.
    /// </summary>
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

    public bool Valid(int i) => i >= 0 && i < _slots.Count;
    
    public InventorySlot GetSlot(int index)
    {
        if (!Valid(index))
            return null;
        return _slots[index];
    }

    public int IndexOfSlot(InventorySlot slot)
    {
        if (slot == null)
            return -1;

        for (int i = 0; i < _slots.Count; i++)
        {
            if (ReferenceEquals(_slots[i], slot))
                return i;
        }

        return -1;
    }

    public bool AddItem(ItemData item, int amount)
    {
        if (item.Stackable && _allowStacking)
        {
            foreach (var slot in _slots)
            {
                if (slot.Item == item && slot.Count < item.MaxStack)
                {
                    int space = item.MaxStack - slot.Count;
                    int add = Mathf.Min(space, amount);
                    slot.Count += add;
                    amount -= add;
                    if (amount <= 0)
                    {
                        InventoryEvents.InventoryChanged?.Invoke();
                        return true;
                    }
                }
            }
        }

        foreach (var slot in _slots)
        {
            if (slot.Item == null && amount > 0)
            {
                int add = _allowStacking && item.Stackable
                    ? Mathf.Min(item.MaxStack, amount)
                    : 1;

                slot.Item = item;
                slot.Count = add;
                amount -= add;
            }
        }

        InventoryEvents.InventoryChanged?.Invoke();

        return amount <= 0;
    }

    public bool AddItem(ItemData item)
    {
        return AddItem(item, 1);
    }

    /// <summary>
    /// Checks whether AddItem would succeed for this amount without changing state.
    /// </summary>
    public bool CanAddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return true;

        int remaining = amount;

        if (item.Stackable && _allowStacking)
        {
            foreach (var slot in _slots)
            {
                if (slot.Item == item && slot.Count < item.MaxStack)
                {
                    int space = item.MaxStack - slot.Count;
                    remaining -= Mathf.Min(space, remaining);
                    if (remaining <= 0)
                        return true;
                }
            }
        }

        foreach (var slot in _slots)
        {
            if (slot.Item == null && remaining > 0)
            {
                int add = _allowStacking && item.Stackable
                    ? Mathf.Min(item.MaxStack, remaining)
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
        if (slot.Item == null)
            return;
        if (amount <= 0)
            return;

        slot.Count -= amount;

        if (slot.Count <= 0)
        {
            slot.Item = null;
            slot.Count = 0;
        }

        InventoryEvents.InventoryChanged?.Invoke();
    }

    public void RemoveItem(InventorySlot slot)
    {
        RemoveItem(slot, 1);
    }

    public void RemoveItem(int index, int amount)
    {
        if (!Valid(index))
            return;
        RemoveItem(_slots[index], amount);
    }

    public void RemoveItem(int index)
    {
        RemoveItem(index, 1);
    }

    /// <summary>
    /// Tries to swap or stack two _slots. Respects maxStack when stacking. Return true if the operation succeeded.
    /// </summary>
    public bool TrySwapOrStack(int fromIndex, int toIndex)
    {
        if (!Valid(fromIndex) || !Valid(toIndex) || fromIndex == toIndex)
            return false;

        var a = _slots[fromIndex];
        var b = _slots[toIndex];

        bool exchangedCells = false;

        if (a.Item != null && b.Item != null && a.Item == b.Item && a.Item.Stackable && _allowStacking)
        {
            int space = a.Item.MaxStack - b.Count;
            if (space <= 0)
            {
                SwapSlots(a, b);
                exchangedCells = true;
            }
            else
            {
                int move = Math.Min(space, a.Count);
                b.Count += move;
                a.Count -= move;
                if (a.Count <= 0)
                {
                    a.Item = null;
                    a.Count = 0;
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

    /// <summary>
    /// Fills the given list with slot indices that pass the current filter.
    /// Reuses the list to avoid GC allocation.
    /// </summary>
    public void GetFilteredSlotIndices(List<int> result)
    {
        if (result == null)
            return;
        _filterState.GetFilteredSlotIndices(_slots, result);
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

    /// <summary>
    /// True when empty grid cells should stay visible: no category filter and no active search text.
    /// </summary>
    public bool ShouldShowEmptySlot()
    {
        return _filterState.ShouldShowEmptySlot();
    }

    /// <summary>
    /// True if passes the current category and search filters.
    /// </summary>
    public bool PassFilter(InventorySlot slot)
    {
        return _filterState.PassFilter(slot);
    }

    /// <summary>
    /// Sets the active sort key and order, reorders filled slots.
    /// </summary>
    public void SetSort(InventorySortType type, SortOrder order)
    {
        _currentSortType = type;
        _currentSortOrder = order;
        ApplySort();
        InventoryEvents.InventoryChanged?.Invoke();
    }

    /// <summary>
    /// Toggles between ascending and descending, then re-sorts.
    /// </summary>
    public void ToggleSortOrder()
    {
        _currentSortOrder = _currentSortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        ApplySort();
        InventoryEvents.InventoryChanged?.Invoke();
    }

    /// <summary>
    /// Creates serializable inventory save data.
    /// </summary>
    public InventorySaveData ToSaveData()
    {
        return InventorySaveDataMapper.ToSaveData(_slots);
    }

    /// <summary>
    /// Loads inventory save data.
    /// </summary>
    public void LoadFromSaveData(InventorySaveData data)
    {
        LoadFromSaveData(data, ItemDatabase.Instance);
    }

    /// <summary>
    /// Loads inventory save data.
    /// </summary>
    public void LoadFromSaveData(InventorySaveData data, IItemDatabase itemDatabase)
    {
        InventorySaveDataMapper.LoadFromSaveData(data, _slots, itemDatabase);
        InventoryEvents.InventoryChanged?.Invoke();
    }

    private void OnItemAddedHandler(ItemData item, int amount)
    {
        if (!AddItem(item, amount))
            Debug.Log("Could not add item — inventory full.");
    }

    private void UseSlot(int index)
    {
        if (!Valid(index))
            return;
        UseSlot(_slots[index], index);
    }

    private void UseSlot(InventorySlot slot)
    {
        int index = IndexOfSlot(slot);
        UseSlot(slot, index);
    }

    private void UseSlot(InventorySlot slot, int slotIndex)
    {
        if (slot == null || slot.Item == null)
            return;

        var ctx = new ItemUseContext(this, _equipmentManager, slotIndex);

        if (_useHandlerRegistry.TryUse(ctx, slot))
            return;

        Debug.Log($"Used item (no handler): {slot.Item.ItemName}");
    }

    private void InspectItem(int index)
    {
        if (!Valid(index))
            return;

        var item = _slots[index].Item;
        if (item == null)
            return;

        Debug.Log($"{item.ItemName}\n{item.Description}");
    }

    private void ApplySort()
    {
        InventorySortUtility.SortSlots(_slots, _currentSortType, _currentSortOrder, _sortFilled, _sortEmpty);
    }

    private void HandleSplitStack(int index)
    {
        if (!Valid(index))
            return;
        if (!_allowSplitStack)
            return;

        var slot = _slots[index];
        if (slot.Item == null)
            return;
        if (!slot.Item.Stackable)
            return;
        if (slot.Count < 2)
            return;

        int half = slot.Count / 2;

        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].Item == null)
            {
                _slots[i].Item = slot.Item;
                _slots[i].Count = half;

                slot.Count -= half;

                InventoryEvents.InventoryChanged?.Invoke();
                return;
            }
        }
    }

    private static void SwapSlots(InventorySlot a, InventorySlot b)
    {
        var tempItem = a.Item;
        var tempCount = a.Count;
        a.Item = b.Item;
        a.Count = b.Count;
        b.Item = tempItem;
        b.Count = tempCount;
    }
}