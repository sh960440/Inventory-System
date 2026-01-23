using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new List<InventorySlot>();
    //public InventorySlotUI[] slotsUI;
    public int initialCapacity = 9;

    void Awake()
    {
        // If there is no default slot，create empty slots for UI to use
        if (slots.Count == 0)
        {
            for (int i = 0; i < initialCapacity; i++)
                slots.Add(new InventorySlot());
        }

        GameEvents.OnItemUsed += UseItem;
        GameEvents.OnItemDropped += DropItem;
        GameEvents.OnItemPicked += OnItemPickedHandler;
    }

    /*void Start()
    {
        // 綁定 UI → InventorySlotUI 會自動找 Inventory
        for (int i = 0; i < slotsUI.Length; i++)
        {
            slotsUI[i].Setup(this, i);
        }

        //UpdateUI();
        GameEvents.OnInventoryChanged?.Invoke();
    }*/

    private void OnDestroy()
    {
        GameEvents.OnItemUsed -= UseItem;
        GameEvents.OnItemDropped -= DropItem;
        GameEvents.OnItemPicked -= OnItemPickedHandler;
    }

    // Action<T> doesn't return a value, so the OnItemPicked event is bound to this handler instead of AddItem.
    private void OnItemPickedHandler(ItemData item, int amount)
    {
        bool success = AddItem(item, amount);
        
        if (!success)
        {
            Debug.Log("Inventory full");
        }
    }

    public bool AddItem(ItemData item, int amount)
    {
        // If stackable
        if (item.stackable)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].item == item && slots[i].count < item.maxStack)
                {
                    int space = item.maxStack - slots[i].count;
                    int toAdd = Mathf.Min(space, amount);

                    slots[i].count += toAdd;
                    amount -= toAdd;

                    if (amount <= 0)
                    {
                        //UpdateUI();
                        GameEvents.OnInventoryChanged?.Invoke();
                        return true;
                    }
                }
            }
        }

        // Find an empty slot
        for (int i = 0; i < slots.Count && amount > 0; i++)
        {
            if (slots[i].item == null)
            {
                int toAdd = Mathf.Min(item.maxStack, amount);
                slots[i].item = item;
                slots[i].count = toAdd;
                amount -= toAdd;
            }
        }

        //UpdateUI();
        GameEvents.OnInventoryChanged?.Invoke();
        // True: Item(s) added successfully. False: Inventory is full.
        return amount <= 0;
    }

    public bool AddItem(ItemData item)
    {
        return AddItem(item, 1);
    }

    /*public void UpdateUI()
    {
        for (int i = 0; i < slotsUI.Length; i++)
        {
            slotsUI[i].Refresh();
        }
    }*/

    // ---------------------------------------------------------
    // Use Item
    // ---------------------------------------------------------
    public void UseItem(int slotIndex)
    {
        if (!Valid(slotIndex)) return;

        var slot = slots[slotIndex];
        var item = slot.item;

        if (item == null) return;

        Debug.Log("Use: " + item.itemName);

        if (item.consumable)
        {
            slot.count--;
            if (slot.count <= 0)
            {
                slot.item = null;
                slot.count = 0;
            }
        }
        else
        {
            Debug.Log(item.itemName + " is non-consumable.");
        }

        //UpdateUI();
        GameEvents.OnInventoryChanged?.Invoke();
    }

    // ---------------------------------------------------------
    // Drop Item (Generate worldPrefab)
    // ---------------------------------------------------------
    public void DropItem(int slotIndex)
    {
        if (!Valid(slotIndex)) return;

        var slot = slots[slotIndex];
        var item = slot.item;

        if (item == null) return;

        Debug.Log("Drop: " + item.itemName);

        // If there's a worldPrefab
        if (item.worldPrefab != null)
        {
            Transform p = GetComponentInParent<Transform>();
            Vector3 pos = p.position + p.forward * 1.2f + Vector3.up * 0.5f;
            Instantiate(item.worldPrefab, pos, Quaternion.identity);
        }

        // Minus 1 from the inventory
        if (item.stackable)
        {
            slot.count--;
            if (slot.count <= 0)
            {
                slot.item = null;
                slot.count = 0;
            }
        }
        else
        {
            slot.item = null;
            slot.count = 0;
        }

        //UpdateUI();
        GameEvents.OnInventoryChanged?.Invoke();
    }

    public void InspectItem(int slotIndex)
    {
        if (!Valid(slotIndex)) return;

        var slot = slots[slotIndex];
        if (slot.item == null) return;

        Debug.Log($"Inspect: {slot.item.itemName}\n{slot.item.description}");
    }

    // ---------------------------------------------------------
    bool Valid(int i)
    {
        return i >= 0 && i < slots.Count;
    }
}
