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
        if (inventory == null) return;
        if (!ValidInventoryIndex(inventory, inventorySlotIndex)) return;

        slots[hotbarIndex].inventory = inventory;
        slots[hotbarIndex].inventorySlotIndex = inventorySlotIndex;

        GameEvents.OnHotbarChanged?.Invoke();
    }

    public void Clear(int hotbarIndex)
    {
        if (!ValidHotbarIndex(hotbarIndex)) return;

        slots[hotbarIndex].Clear();
        GameEvents.OnHotbarChanged?.Invoke();
    }

    public InventorySlot GetInventorySlot(int hotbarIndex)
    {
        if (!ValidHotbarIndex(hotbarIndex)) return null;

        var slot = slots[hotbarIndex];
        if (slot.inventory == null) return null;

        int invIndex = slot.inventorySlotIndex;
        if (!ValidInventoryIndex(slot.inventory, invIndex)) return null;

        return slot.inventory.slots[invIndex];
    }

    // Validation
    void ValidateSlots()
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty) continue;

            if (slot.inventory == null)
            {
                slot.Clear();
                continue;
            }

            int i = slot.inventorySlotIndex;
            if (!ValidInventoryIndex(slot.inventory, i))
            {
                slot.Clear();
                continue;
            }

            var invSlot = slot.inventory.GetSlot(i);
            if (invSlot == null || invSlot.item == null)
            {
                slot.Clear();
                continue;
            }
        }

        GameEvents.OnHotbarChanged?.Invoke();
    }

    // Helpers
    public bool ValidHotbarIndex(int i) =>
        i >= 0 && i < slots.Count;

    public bool ValidInventoryIndex(Inventory inv, int i) =>
        inv != null && i >= 0 && i < inv.slots.Count;
}
