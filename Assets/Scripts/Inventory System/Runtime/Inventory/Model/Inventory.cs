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
    public InventorySortType currentSortType = InventorySortType.None;
    public SortOrder currentSortOrder = SortOrder.Ascending;
    private bool allowStacking = true;
    private bool allowSplitStack = true;
    private Equipment equipmentManager;
    readonly List<IItemUseHandler> useHandlers = new List<IItemUseHandler>(2);

    public bool IsOpen { get; private set; }
    public bool AllowDoubleClickUse { get; private set; }

    public void SetOpen(bool open)
    {
        if (IsOpen == open)
            return;

        IsOpen = open;

        if (open)
            InventoryEvents.InventoryToggled?.Invoke(true);
        else
            InventoryEvents.InventoryClosed?.Invoke();
    }

    bool IsAllCategory =>
        currentCategories == null || currentCategories.Length == 0;

    bool HasSearch =>
        !string.IsNullOrEmpty(currentSearch);

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
        InventoryEvents.ItemRemoved += RemoveItem;
        InventoryEvents.ItemInspected += InspectItem;
        InventoryEvents.ItemAdded += OnItemAddedHandler;
        InventoryEvents.HotbarUseRequested += UseSlot;
        InventoryEvents.SplitStackRequested += HandleSplitStack;
    }

    void OnDisable()
    {
        InventoryEvents.ItemUsed -= UseSlot;
        InventoryEvents.ItemRemoved -= RemoveItem;
        InventoryEvents.ItemInspected -= InspectItem;
        InventoryEvents.ItemAdded -= OnItemAddedHandler;
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

    bool PassCategory(InventorySlot slot)
    {
        // All category = Always pass
        if (currentCategories == null || currentCategories.Length == 0)
            return true;

        if (slot.item == null)
            return false;

        foreach (var c in currentCategories)
        {
            if (slot.item.category == c)
                return true;
        }

        return false;
    }

    bool PassSearch(InventorySlot slot)
    {
        if (string.IsNullOrEmpty(currentSearchLower))
            return true;

        if (slot.item == null)
            return false;

        var itemName = slot.item.itemName;
        /* var description = slot.item.description; */ // TBD

        return
            (!string.IsNullOrEmpty(itemName) && itemName.IndexOf(currentSearchLower, StringComparison.OrdinalIgnoreCase) >= 0)/* ||
            (!string.IsNullOrEmpty(description) && description.IndexOf(currentSearchLower, StringComparison.OrdinalIgnoreCase) >= 0)*/; // TBD
    }

    // An API for UI
    public List<int> GetFilteredSlotIndices()
    {
        var result = new List<int>();

        for (int i = 0; i < slots.Count; i++)
        {
            if (PassFilter(slots[i]))
                result.Add(i);
        }

        return result;
    }

    public void GetFilteredSlotIndices(List<int> result)
    {
        if (result == null) return;
        result.Clear();

        for (int i = 0; i < slots.Count; i++)
        {
            if (PassFilter(slots[i]))
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
        return IsAllCategory && !HasSearch;
    }

    public bool PassFilter(InventorySlot slot)
    {
        if (slot.item == null)
            return ShouldShowEmptySlot();

        if (!PassCategory(slot)) return false;
        if (!PassSearch(slot)) return false;

        return true;
    }

    public void SetSort(InventorySortType type, SortOrder order)
    {
        currentSortType = type;
        currentSortOrder = order;
        ApplySort();
        InventoryEvents.InventoryChanged?.Invoke();
    }

    void ApplySort()
    {
        // Identify which slots are occupied and which are empty
        var filled = new List<InventorySlot>();
        var empty = new List<InventorySlot>();

        foreach (var s in slots)
        {
            if (s.item == null) empty.Add(s);
            else filled.Add(s);
        }

        // Sort slots with items
        filled.Sort(CompareSlots);

        slots.Clear();
        slots.AddRange(filled);
        slots.AddRange(empty);
    }

    int CompareSlots(InventorySlot a, InventorySlot b)
    {
        if (a.item == null || b.item == null)
            return 0;

        int result = 0;

        switch (currentSortType)
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

        if (currentSortOrder == SortOrder.Descending)
            result = -result;

        return result;
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