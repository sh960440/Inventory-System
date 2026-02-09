using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    [Header("Config")]
    public int hotbarSize = 6;

    public List<HotbarSlot> slots = new();

    void Awake()
    {
        InitSlots();
    }

    void OnEnable()
    {
        GameEvents.OnInventoryChanged += ValidateSlots;
    }

    void OnDisable()
    {
        GameEvents.OnInventoryChanged -= ValidateSlots;
    }

    void InitSlots()
    {
        slots.Clear();
        for (int i = 0; i < hotbarSize; i++)
            slots.Add(new HotbarSlot());
    }

    public void Assign(int hotbarIndex, Inventory inventory, int inventorySlotIndex)
    {
        if (!ValidHotbarIndex(hotbarIndex)) return;
        if (!ValidInventoryIndex(inventory, inventorySlotIndex)) return;

        var invSlot = inventory.GetSlot(inventorySlotIndex);
        if (invSlot == null || invSlot.item == null) return;

        var hb = slots[hotbarIndex];
        hb.inventory = inventory;
        hb.item = invSlot.item;
        hb.boundInventorySlotIndex = inventorySlotIndex;

        GameEvents.OnHotbarChanged?.Invoke();
    }

    public void Clear(int hotbarIndex)
    {
        if (!ValidHotbarIndex(hotbarIndex)) return;

        slots[hotbarIndex].Clear();
        GameEvents.OnHotbarChanged?.Invoke();
    }

    public void Swap(int a, int b)
    {
        if (!ValidHotbarIndex(a) || !ValidHotbarIndex(b)) return;

        (slots[a], slots[b]) = (slots[b], slots[a]);
        GameEvents.OnHotbarChanged?.Invoke();
    }

    //public InventorySlot GetInventorySlot(int hotbarIndex)
    //{
    //    if (!ValidHotbarIndex(hotbarIndex)) return null;

    //    var slot = slots[hotbarIndex];
    //    if (slot.inventory == null) return null;

    //    int invIndex = slot.inventorySlotIndex;
    //    if (!ValidInventoryIndex(slot.inventory, invIndex)) return null;

    //    return slot.inventory.slots[invIndex];
    //}
    public InventorySlot GetInventorySlot(int hotbarIndex)
    {
        if (!ValidHotbarIndex(hotbarIndex)) return null;

        var hb = slots[hotbarIndex];
        if (hb.inventory == null || hb.item == null) return null;

        // Try using the original binding index
        if (hb.boundInventorySlotIndex >= 0)
        {
            var s = hb.inventory.GetSlot(hb.boundInventorySlotIndex);
            if (s != null && s.item == hb.item)
                return s;
        }

        // fallback: search inventory
        for (int i = 0; i < hb.inventory.SlotCount; i++)
        {
            var s = hb.inventory.GetSlot(i);
            if (s.item == hb.item)
            {
                hb.boundInventorySlotIndex = i;
                return s;
            }
        }

        // Unable to find = inventory no longer has the item, clear hotbar slot
        hb.Clear();
        return null;
    }

    // Validation
    void ValidateSlots()
    {
        foreach (var hb in slots)
        {
            if (hb.IsEmpty) continue;
            if (hb.inventory == null) { hb.Clear(); continue; }

            if (GetInventorySlot(slots.IndexOf(hb)) == null)
                hb.Clear();
        }

        GameEvents.OnHotbarChanged?.Invoke();
    }

    //void ValidateSlots()
    //{
    //    foreach (var slot in slots)
    //    {
    //        if (slot.IsEmpty) continue;

    //        if (slot.inventory == null)
    //        {
    //            slot.Clear();
    //            continue;
    //        }

    //        int i = slot.inventorySlotIndex;
    //        if (!ValidInventoryIndex(slot.inventory, i))
    //        {
    //            slot.Clear();
    //            continue;
    //        }

    //        var invSlot = slot.inventory.GetSlot(i);
    //        if (invSlot == null || invSlot.item == null)
    //        {
    //            slot.Clear();
    //            continue;
    //        }
    //    }

    //    GameEvents.OnHotbarChanged?.Invoke();
    //}

    // Helpers
    public bool ValidHotbarIndex(int i) =>
        i >= 0 && i < slots.Count;

    public bool ValidInventoryIndex(Inventory inv, int i) =>
        inv != null && i >= 0 && i < inv.slots.Count;
}
