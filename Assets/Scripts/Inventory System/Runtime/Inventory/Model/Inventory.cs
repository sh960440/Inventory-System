using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new();
    public int initialCapacity;
    public ItemCategory[] currentCategories;
    string currentSearch = "";
    string currentSearchLower = "";
    private InventorySortType currentSortType = InventorySortType.None;
    private SortOrder currentSortOrder = SortOrder.Ascending;

    public InventorySortType CurrentSortType => currentSortType;
    public SortOrder CurrentSortOrder => currentSortOrder;
    private bool allowStacking = true;
    private bool allowSplitStack = true;
    private Equipment equipmentManager;
    readonly List<IItemUseHandler> useHandlers = new List<IItemUseHandler>(2);
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
        initialCapacity = config.inventoryRows * config.inventoryColumns;

        if (slots.Count == 0)
        {
            for (int i = 0; i < initialCapacity; i++)
                slots.Add(new InventorySlot());
        }

        equipmentManager = em;
        allowStacking = config.allowStacking;
        allowSplitStack = config.allowSplitStack;
        AllowDoubleClickUse = config.allowInventoryDoubleClickUse;

        EnsureDefaultUseHandlers();
    }

    public void ClearUseHandlers()
    {
        useHandlers.Clear();
    }

    public void RegisterUseHandler(IItemUseHandler handler)
    {
        if (handler == null) return;
        useHandlers.Add(handler);
    }

    public void EnsureUseHandlers()
    {
        EnsureDefaultUseHandlers();
    }

    void EnsureDefaultUseHandlers()
    {
        if (useHandlers.Count > 0)
            return;

        useHandlers.Add(new ConsumableUseHandler());
        useHandlers.Add(new EquipmentUseHandler());
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

        if (a.item != null && b.item != null && a.item == b.item && a.item.stackable && allowStacking)
        {
            int space = a.item.maxStack - b.count;
            if (space <= 0)
            {
                SwapSlots(a, b); // Target full, swap instead
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
        }

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
        UseSlot(slots[index]);
    }

    void UseSlot(InventorySlot slot)
    {
        if (slot == null || slot.item == null)
            return;

        EnsureDefaultUseHandlers();

        var ctx = new ItemUseContext(this, equipmentManager);
        var item = slot.item;

        for (int i = 0; i < useHandlers.Count; i++)
        {
            var h = useHandlers[i];
            if (h != null && h.CanUse(item))
            {
                h.Use(ctx, slot);
                return;
            }
        }

        Debug.Log($"Used item (no handler): {slot.item.itemName}");
    }

    //public void DropItem(int index, int amount)
    //{
    //    if (!Valid(index)) return;

    //    var slot = slots[index];
    //    if (slot.item == null) return;
    //    if (amount <= 0) return;

    //    int dropCount = Mathf.Min(amount, slot.count);

    //    // Get the world prefab of the item and instantiate it in front of the player
    //    if (slot.item.worldPrefab != null)
    //    {
    //        var t = transform;
    //        for (int i = 0; i < dropCount; i++)
    //        {
    //            Instantiate(
    //                slot.item.worldPrefab,
    //                t.position + t.forward * 1.2f + Vector3.up * 2.0f,
    //                Quaternion.identity
    //            );
    //        }
    //    }

    //    RemoveItem(slot, dropCount);
    //    InventoryEvents.InventoryChanged?.Invoke();
    //}

    //void DropItem(int index)
    //{
    //    DropItem(index, 1);
    //}

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
        result.Clear();

        var showEmpty = InventoryFilterUtility.ShouldShowEmptySlot(currentCategories, currentSearch);
        for (int i = 0; i < slots.Count; i++)
        {
            if (InventoryFilterUtility.PassFilter(slots[i], currentCategories, currentSearchLower, showEmpty))
                result.Add(i);
        }
    }

    public void SetCategoryFilter(ItemCategory[] categories)
    {
        currentCategories = categories;
        InventoryEvents.InventoryChanged?.Invoke();
    }

    public void SetSearchKeyword(string keyword)
    {
        currentSearch = keyword ?? "";
        currentSearchLower = currentSearch.Trim();
        InventoryEvents.InventoryChanged?.Invoke();
    }

    public bool ShouldShowEmptySlot()
    {
        return InventoryFilterUtility.ShouldShowEmptySlot(currentCategories, currentSearch);
    }

    public bool PassFilter(InventorySlot slot)
    {
        return InventoryFilterUtility.PassFilter(
            slot,
            currentCategories,
            currentSearchLower,
            ShouldShowEmptySlot());
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
        var data = new InventorySaveData();

        foreach (var slot in slots)
        {
            if (slot.item == null)
            {
                data.slots.Add(new InventorySlotSaveData
                {
                    itemId = null,
                    count = 0
                });
            }
            else
            {
                data.slots.Add(new InventorySlotSaveData
                {
                    itemId = slot.item.Id,
                    count = slot.count
                });
            }
        }

        return data;
    }

    public void LoadFromSaveData(InventorySaveData data)
    {
        LoadFromSaveData(data, ItemDatabase.Instance);
    }
    public void LoadFromSaveData(InventorySaveData data, IItemDatabase itemDatabase)
    {
        slots.Clear();

        foreach (var s in data.slots)
        {
            if (string.IsNullOrEmpty(s.itemId))
            {
                slots.Add(new InventorySlot());
            }
            else
            {
                var item = itemDatabase?.Get(s.itemId);

                slots.Add(
                    item != null
                        ? new InventorySlot(item, s.count)
                        : new InventorySlot()
                );
            }
        }

        InventoryEvents.InventoryChanged?.Invoke();
    }
}