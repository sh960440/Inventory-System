using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runtime hotbar bindings: each slot points at an inventory cell for quick use.
/// </summary>
public class Hotbar : MonoBehaviour
{
    private readonly List<HotbarSlot> _slots = new List<HotbarSlot>();
    
    private Equipment _equipment;

    public bool AllowDoubleClickUse { get; private set; }

    private void OnEnable()
    {
        InventoryEvents.InventoryChanged += ValidateSlots;
        InventoryEvents.InventorySlotsSwapped += OnInventorySlotsSwapped;
    }

    private void OnDisable()
    {
        InventoryEvents.InventoryChanged -= ValidateSlots;
        InventoryEvents.InventorySlotsSwapped -= OnInventorySlotsSwapped;
    }

    /// <summary>
    /// Sets the hotbar size and applies interaction flags from config.
    /// </summary>
    public void ApplyConfig(ItemSystemConfiguration config, Equipment equipment = null)
    {
        _equipment = equipment;

        _slots.Clear();
        for (int i = 0; i < config.HotkeyCount; i++)
            _slots.Add(new HotbarSlot());

        AllowDoubleClickUse = config.AllowHotbarDoubleClickUse;
    }

    /// <summary>
    /// Binds a hotbar cell to an inventory slot.
    /// </summary>
    public void Assign(int hotbarIndex, Inventory inventory, int inventorySlotIndex)
    {
        if (!ValidHotbarIndex(hotbarIndex))
            return;
        if (!ValidInventoryIndex(inventory, inventorySlotIndex))
            return;

        var invSlot = inventory.GetSlot(inventorySlotIndex);
        if (invSlot == null || invSlot.Item == null)
            return;

        var hb = _slots[hotbarIndex];
        hb.Inventory = inventory;
        hb.Item = invSlot.Item;
        hb.BoundInventoryCell = invSlot;

        InventoryEvents.HotbarChanged?.Invoke();
    }

    /// <summary>
    /// Clears a hotbar binding.
    /// </summary>
    public void Clear(int hotbarIndex)
    {
        if (!ValidHotbarIndex(hotbarIndex))
            return;

        _slots[hotbarIndex].Clear();
        InventoryEvents.HotbarChanged?.Invoke();
    }

    /// <summary>
    /// Swaps two hotbar bindings.
    /// </summary>
    public void Swap(int a, int b)
    {
        if (!ValidHotbarIndex(a) || !ValidHotbarIndex(b))
            return;

        var temp = _slots[a];
        _slots[a] = _slots[b];
        _slots[b] = temp;

        InventoryEvents.HotbarChanged?.Invoke();
    }

    /// <summary>
    /// Gets the InventorySlot linked to the hotbar index, clearing the binding if the item is no longer valid.
    /// </summary>
    public InventorySlot GetInventorySlot(int hotbarIndex)
    {
        if (!ValidHotbarIndex(hotbarIndex))
            return null;

        var hb = _slots[hotbarIndex];
        if (hb.Inventory == null || hb.Item == null)
            return null;

        if (hb.Inventory is not Inventory invTyped)
            return null;

        if (hb.BoundInventoryCell != null)
        {
            int idx = invTyped.IndexOfSlot(hb.BoundInventoryCell);
            if (idx >= 0 && hb.BoundInventoryCell.Item != null && hb.BoundInventoryCell.Item == hb.Item)
                return hb.BoundInventoryCell;

            var relocated = TryRelocateItemCell(invTyped, hb);
            if (relocated != null)
            {
                hb.BoundInventoryCell = relocated;
                return relocated;
            }

            hb.Clear();
            return null;
        }

        var found = TryRelocateItemCell(invTyped, hb);
        if (found != null)
        {
            hb.BoundInventoryCell = found;
            return found;
        }

        // Unable to find = inventory no longer has the item, clear hotbar slot
        hb.Clear();
        return null;
    }

    /// <summary>
    /// Returns the inventory slot index bound to the specified hotbar entry, or -1 if none is assigned.
    /// </summary>
    public int GetBoundInventorySlotIndex(int hotbarIndex)
    {
        if (!ValidHotbarIndex(hotbarIndex))
            return -1;

        var hb = _slots[hotbarIndex];
        if (hb.Inventory is not Inventory inv || hb.BoundInventoryCell == null)
            return -1;

        int idx = inv.IndexOfSlot(hb.BoundInventoryCell);
        return idx;
    }

    /// <summary>Returns true if i is a valid hotbar index.</summary>
    public bool ValidHotbarIndex(int i) => i >= 0 && i < _slots.Count;

    /// <summary>Returns true if i is a valid index into inv.</summary>
    public bool ValidInventoryIndex(Inventory inv, int i) =>
        inv != null && i >= 0 && i < inv.SlotCount;

    /// <summary>
    /// Creates serializable hotbar save data.
    /// </summary>
    public HotbarSaveData ToSaveData()
    {
        var data = new HotbarSaveData();

        foreach (var slot in _slots)
        {
            if (slot.IsEmpty)
            {
                data.itemIds.Add(new HotbarSlotSaveData { itemId = null, inventorySlotIndex = -1 });
                continue;
            }

            int idx = -1;
            if (slot.Inventory is Inventory inv && slot.BoundInventoryCell != null)
                idx = inv.IndexOfSlot(slot.BoundInventoryCell);

            data.itemIds.Add(new HotbarSlotSaveData
            {
                itemId = slot.Item != null ? slot.Item.Id : null,
                inventorySlotIndex = idx
            });
        }

        return data;
    }

    /// <summary>
    /// Loads hotbar save data.
    /// </summary>
    public void LoadFromSaveData(HotbarSaveData data, Inventory inventory)
    {
        LoadFromSaveData(data, inventory, ItemDatabase.Instance);
    }

    /// <summary>
    /// Loads hotbar save data.
    /// </summary>
    public void LoadFromSaveData(HotbarSaveData data, Inventory inventory, IItemDatabase itemDatabase)
    {
        if (data == null || data.itemIds == null)
            data = new HotbarSaveData();

        for (int i = 0; i < _slots.Count; i++)
            _slots[i].Clear();

        for (int i = 0; i < data.itemIds.Count && i < _slots.Count; i++)
        {
            var entry = data.itemIds[i];
            if (string.IsNullOrEmpty(entry.itemId))
                continue;

            var targetItem = itemDatabase?.Get(entry.itemId);
            if (targetItem == null)
                continue;

            if (ItemInventoryBindingSaveData.IsBindingValid(inventory, itemDatabase, entry.itemId, entry.inventorySlotIndex))
            {
                Assign(i, inventory, entry.inventorySlotIndex);
                continue;
            }

            for (int invIndex = 0; invIndex < inventory.SlotCount; invIndex++)
            {
                var invSlot = inventory.GetSlot(invIndex);

                if (invSlot.Item == targetItem)
                {
                    Assign(i, inventory, invIndex);
                    break;
                }
            }
        }

        InventoryEvents.HotbarChanged?.Invoke();
    }

    /// <summary>
    /// Updates hotbar bindings after two inventory slots swap their item contents.
    /// </summary>
    private void OnInventorySlotsSwapped(int fromIndex, int toIndex)
    {
        foreach (var hb in _slots)
        {
            if (hb.IsEmpty || hb.Inventory is not Inventory inv || hb.BoundInventoryCell == null)
                continue;

            var fromSlot = inv.GetSlot(fromIndex);
            var toSlot = inv.GetSlot(toIndex);

            if (!ReferenceEquals(hb.BoundInventoryCell, fromSlot) && !ReferenceEquals(hb.BoundInventoryCell, toSlot))
                continue;

            if (hb.BoundInventoryCell.Item != null && hb.BoundInventoryCell.Item == hb.Item)
                continue;

            var relocated = TryRelocateItemCell(inv, hb);
            if (relocated != null)
                hb.BoundInventoryCell = relocated;
            else
                hb.Clear();
        }

        InventoryEvents.HotbarChanged?.Invoke();
    }

    private static InventorySlot TryRelocateItemCell(Inventory invTyped, HotbarSlot hb, Equipment equipment)
    {
        if (equipment != null && hb.Item is EquipmentData eq)
        {
            var src = equipment.GetEquippedSourceInventorySlot(eq);
            if (src != null && src.Item == hb.Item && invTyped.IndexOfSlot(src) >= 0)
                return src;
        }

        for (int i = 0; i < invTyped.SlotCount; i++)
        {
            var s = invTyped.GetSlot(i);
            if (s.Item == hb.Item)
                return s;
        }

        return null;
    }

    private InventorySlot TryRelocateItemCell(Inventory invTyped, HotbarSlot hb) =>
        TryRelocateItemCell(invTyped, hb, _equipment);

    private void ValidateSlots()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            var hb = _slots[i];
            if (hb.IsEmpty)
                continue;
            if (hb.Inventory == null)
            {
                hb.Clear();
                continue;
            }

            if (GetInventorySlot(i) == null)
                hb.Clear();
        }

        InventoryEvents.HotbarChanged?.Invoke();
    }
}