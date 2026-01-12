using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new List<InventorySlot>();
    public InventorySlotUI[] slotsUI;
    public int initialCapacity = 9;

    void Awake()
    {
        // If there is no default slot，create empty slots for UI to use
        if (slots.Count == 0)
        {
            for (int i = 0; i < initialCapacity; i++)
                slots.Add(new InventorySlot());
        }
    }

    public void AddItem(ItemData data, int amount = 1)
    {
        if (data == null) return;

        // Look for stackable items
        if (data.stackable)
        {
            foreach (var s in slots)
            {
                if (s.item == data)
                {
                    s.count += amount;
                    UpdateUI();
                    Debug.Log($"Stacked {data.itemName}. New count: {s.count}");
                    return;
                }
            }
        }

        // Find the first empty slot
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = data;
                slots[i].count = amount;
                UpdateUI();
                    Debug.Log($"Added {data.itemName} to slot {i}");
                return;
            }
        }

        // If it's full, extend
        var newSlot = new InventorySlot { item = data, count = amount };
        slots.Add(newSlot);
        Debug.Log($"Inventory full: auto-expanded and added {data.itemName}");
    }

    public void UpdateUI()
    {
        // If the amount of slotUI doesn't match the amount of slots
        int len = Mathf.Min(slotsUI.Length, slots.Count);
        for (int i = 0; i < len; i++)
        {
            var dataSlot = slots[i];
            if (dataSlot != null && dataSlot.item != null)
                slotsUI[i].SetItem(dataSlot.item.icon);
            else
                slotsUI[i].Clear();
        }
    }
}
