using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new();
    public int initialCapacity = 9;
    public ItemCategory[] currentCategories;
    string currentSearch = "";
    public InventorySortType currentSortType = InventorySortType.None;
    public SortOrder currentSortOrder = SortOrder.Ascending;

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

    void Awake()
    {
        // If there is no default slot，create empty slots for UI to use
        if (slots.Count == 0)
        {
            for (int i = 0; i < initialCapacity; i++)
                slots.Add(new InventorySlot());
        }
    }

    void OnEnable()
    {
        GameEvents.OnItemUsed += UseItem;
        GameEvents.OnItemDropped += DropItem;
        GameEvents.OnItemInspected += InspectItem;
        GameEvents.OnItemPicked += OnItemPickedHandler;
        GameEvents.OnHotbarUseRequested += OnHotbarUseRequested;
    }

    void OnDisable()
    {
        GameEvents.OnItemUsed -= UseItem;
        GameEvents.OnItemDropped -= DropItem;
        GameEvents.OnItemInspected -= InspectItem;
        GameEvents.OnItemPicked -= OnItemPickedHandler;
        GameEvents.OnHotbarUseRequested -= OnHotbarUseRequested;
    }

    void OnItemPickedHandler(ItemData item, int amount)
    {
        if (!AddItem(item, amount))
            Debug.Log("Inventory full");
    }

    public bool AddItem(ItemData item, int amount)
    {
        // If stackable
        if (item.stackable)
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
                        GameEvents.OnInventoryChanged?.Invoke();
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
                int add = Mathf.Min(item.maxStack, amount);
                slot.item = item;
                slot.count = add;
                amount -= add;
            }
        }

        GameEvents.OnInventoryChanged?.Invoke();
        // True: Item(s) added successfully. False: Inventory is full.
        return amount <= 0;
    }

    public bool AddItem(ItemData item)
    {
        return AddItem(item, 1);
    }

    void UseItem(int index)
    {
        if (!Valid(index)) return;
        var slot = slots[index];
        if (slot.item == null) return;

        Debug.Log("Use: " + slot.item.itemName);
        if (slot.item.consumable)
            Consume(slot);
        else
        {
            Debug.Log(slot.item.itemName + " is non-consumable.");
        }

        GameEvents.OnInventoryChanged?.Invoke();
    }

    void Consume(InventorySlot slot)
    {
        Consume(slot, 1);
    }

    void Consume(InventorySlot slot, int amount)
    {
        if (slot.item == null) return;
        if (amount <= 0) return;

        slot.count -= amount;

        if (slot.count <= 0)
        {
            slot.item = null;
            slot.count = 0;
        }
    }

    public void DropItem(int index, int amount)
    {
        if (!Valid(index)) return;

        var slot = slots[index];
        if (slot.item == null) return;
        if (amount <= 0) return;

        int dropCount = Mathf.Min(amount, slot.count);

        // Get the world prefab of the item and instantiate it in front of the player
        if (slot.item.worldPrefab != null)
        {
            var t = transform;
            for (int i = 0; i < dropCount; i++)
            {
                Instantiate(
                    slot.item.worldPrefab,
                    t.position + t.forward * 1.2f - Vector3.up * 1.0f,
                    Quaternion.identity
                );
            }
        }

        Consume(slot, dropCount);
        GameEvents.OnInventoryChanged?.Invoke();
    }


    void DropItem(int index)
    {
        DropItem(index, 1);
    }

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
        if (string.IsNullOrEmpty(currentSearch))
            return true;

        if (slot.item == null)
            return false;

        string q = currentSearch.ToLower();

        return
            slot.item.itemName.ToLower().Contains(q) ||
            slot.item.description.ToLower().Contains(q);
    }

    // An API for UI
    public List<int> GetFilteredSlotIndices()
    {
        var result = new List<int>();

        for (int i = 0; i < slots.Count; i++)
        {
            if (PassCategory(slots[i]))
                result.Add(i);
        }

        return result;
    }

    public void SetCategoryFilter(ItemCategory[] categories)
    {
        currentCategories = categories;
        GameEvents.OnInventoryChanged.Invoke();
    }

    public void SetSearchKeyword(string keyword)
    {
        currentSearch = keyword;
        GameEvents.OnInventoryChanged?.Invoke();
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
        GameEvents.OnInventoryChanged?.Invoke();
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

    void OnHotbarUseRequested(InventorySlot slot)
    {
        if (slot == null || slot.item == null)
            return;

        // Consumable
        if (slot.item.consumable)
        {
            Consume(slot);
            GameEvents.OnInventoryChanged?.Invoke();
            return;
        }

        // Equipment
        if (slot.item is EquipmentData eq)
        {
            if (FindFirstObjectByType<EquipmentManager>()?.IsEquipped(eq) == true)
            {
                GameEvents.OnUnequipRequested?.Invoke(eq.equipSlot);
            }
            else
            {
                GameEvents.OnEquipRequested?.Invoke(eq);
            }

            return;
        }

        Debug.Log($"Hotbar used item: {slot.item.itemName}");
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
        slots.Clear();

        foreach (var s in data.slots)
        {
            if (string.IsNullOrEmpty(s.itemId))
            {
                slots.Add(new InventorySlot());
            }
            else
            {
                var item = ItemDatabase.Instance.Get(s.itemId);

                slots.Add(
                    item != null
                        ? new InventorySlot(item, s.count)
                        : new InventorySlot()
                );
            }
        }

        GameEvents.OnInventoryChanged?.Invoke();
    }
}