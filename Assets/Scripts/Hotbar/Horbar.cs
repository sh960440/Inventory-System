using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    [Header("Config")]
    public int hotbarSize = 6;

    public List<HotbarSlot> slots = new();

    Inventory inventory;

    void Awake()
    {
        inventory = FindFirstObjectByType<Inventory>();
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

    public void Assign(int hotbarIndex, int inventorySlotIndex)
    {
        if (!ValidHotbarIndex(hotbarIndex)) return;
        if (!inventorySlotIndexValid(inventorySlotIndex)) return;

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

        int invIndex = slots[hotbarIndex].inventorySlotIndex;
        if (!inventorySlotIndexValid(invIndex)) return null;

        return inventory.slots[invIndex];
    }

    void ValidateSlots()
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty) continue;

            if (!inventorySlotIndexValid(slot.inventorySlotIndex))
                slot.Clear();
            else if (inventory.slots[slot.inventorySlotIndex].item == null)
                slot.Clear();
        }

        GameEvents.OnHotbarChanged?.Invoke();
    }

    bool ValidHotbarIndex(int i) => i >= 0 && i < slots.Count;
    bool inventorySlotIndexValid(int i) => inventory != null && i >= 0 && i < inventory.slots.Count;

    public void Assign(int hotbarIndex, Inventory inv, int inventorySlotIndex)
    {
        slots[hotbarIndex].inventory = inv;
        slots[hotbarIndex].inventorySlotIndex = inventorySlotIndex;

        GameEvents.OnHotbarChanged?.Invoke();
    }

}
