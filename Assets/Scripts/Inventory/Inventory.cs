using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new();
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

    void OnEnable()
    {
        GameEvents.OnItemUsed += UseItem;
        GameEvents.OnItemDropped += DropItem;
        GameEvents.OnItemInspected += InspectItem;
        GameEvents.OnItemPicked += OnItemPickedHandler;
    }

    void OnDisable()
    {
        GameEvents.OnItemUsed -= UseItem;
        GameEvents.OnItemDropped -= DropItem;
        GameEvents.OnItemInspected -= InspectItem;
        GameEvents.OnItemPicked -= OnItemPickedHandler;
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
        slot.count--;
        if (slot.count <= 0)
        {
            slot.item = null;
            slot.count = 0;
        }
    }

    void DropItem(int index)
    {
        if (!Valid(index)) return;
        var slot = slots[index];
        if (slot.item == null) return;

        Debug.Log("Drop: " + slot.item.itemName);

        if (slot.item.worldPrefab != null)
        {
            var t = transform;
            Instantiate(slot.item.worldPrefab,
                t.position + t.forward * 1.2f + Vector3.up * 0.5f,
                Quaternion.identity);
        }

        Consume(slot);
        GameEvents.OnInventoryChanged?.Invoke();
    }

    void InspectItem(int index)
    {
        if (!Valid(index)) return;
        var item = slots[index].item;
        if (item == null) return;
        Debug.Log($"{item.itemName}\n{item.description}");
    }

    bool Valid(int i) => i >= 0 && i < slots.Count;
}
