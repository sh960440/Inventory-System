using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runtime hotbar bindings: each slot points at an inventory cell for quick use.
/// </summary>
public class Hotbar : MonoBehaviour
{
    private readonly List<HotbarSlot> _slots = new List<HotbarSlot>();

    public bool AllowDoubleClickUse { get; private set; }

    private void OnEnable()
    {
        InventoryEvents.InventoryChanged += ValidateSlots;
    }

    private void OnDisable()
    {
        InventoryEvents.InventoryChanged -= ValidateSlots;
    }

    /// <summary>
    /// Sets the hotbar size and applies interaction flags from config.
    /// </summary>
    public void ApplyConfig(ItemSystemConfiguration config)
    {
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
        hb.BoundInventorySlotIndex = inventorySlotIndex;

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

        // Try using the original binding index
        if (hb.BoundInventorySlotIndex >= 0)
        {
            var s = hb.Inventory.GetSlot(hb.BoundInventorySlotIndex);
            if (s != null && s.Item == hb.Item)
                return s;
        }

        // fallback = search inventory
        for (int i = 0; i < hb.Inventory.SlotCount; i++)
        {
            var s = hb.Inventory.GetSlot(i);
            if (s.Item == hb.Item)
            {
                hb.BoundInventorySlotIndex = i;
                return s;
            }
        }

        // Unable to find = inventory no longer has the item, clear hotbar slot
        hb.Clear();
        return null;
    }

    /// <summary>
    /// Get inventory slot index for this hotbar cell, or -1 when invalid or empty.
    /// </summary>
    public int GetBoundInventorySlotIndex(int hotbarIndex)
    {
        if (!ValidHotbarIndex(hotbarIndex))
            return -1;

        return _slots[hotbarIndex].BoundInventorySlotIndex;
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
            data.itemIds.Add(new HotbarSlotSaveData
            {
                itemId = slot.Item != null ? slot.Item.Id : null
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
        for (int i = 0; i < _slots.Count; i++)
            _slots[i].Clear();

        for (int i = 0; i < data.itemIds.Count && i < _slots.Count; i++)
        {
            var itemId = data.itemIds[i].itemId;
            if (string.IsNullOrEmpty(itemId))
                continue;

            var targetItem = itemDatabase?.Get(itemId);
            if (targetItem == null)
                continue;

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