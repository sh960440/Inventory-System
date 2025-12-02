using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new List<InventorySlot>();

    public void AddItem(ItemData data, int amount)
    {
        // Look for stackable items
        foreach (var slot in slots)
        {
            if (slot.item == data && data.stackable)
            {
                slot.count += amount;
                return;
            }
        }

        // Add a new item slot when there are no stackable items in the inventory
        InventorySlot newSlot = new InventorySlot
        {
            item = data,
            count = amount
        };

        slots.Add(newSlot);

        Debug.Log($"Added {data.itemName} x {amount}");
    }
}
