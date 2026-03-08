using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public List<HotbarSlot> slots = new();
    public bool AllowDoubleClickUse { get; private set; }

    void Awake()
    {
        
    }

    void OnEnable()
    {
        InventoryEvents.InventoryChanged += ValidateSlots;
    }

    void OnDisable()
    {
        InventoryEvents.InventoryChanged -= ValidateSlots;
    }

    public void ApplyConfig(ItemSystemConfiguration config)
    {
        slots.Clear();
        for (int i = 0; i < config.hotkeyCount; i++)
            slots.Add(new HotbarSlot());

        AllowDoubleClickUse = config.allowHotbarDoubleClickUse;
    }
    void InitSlots()
    {
        
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

        InventoryEvents.HotbarChanged?.Invoke();
    }

    public void Clear(int hotbarIndex)
    {
        if (!ValidHotbarIndex(hotbarIndex)) return;

        slots[hotbarIndex].Clear();
        InventoryEvents.HotbarChanged?.Invoke();
    }

    public void Swap(int a, int b)
    {
        if (!ValidHotbarIndex(a) || !ValidHotbarIndex(b)) return;

        (slots[a], slots[b]) = (slots[b], slots[a]);
        InventoryEvents.HotbarChanged?.Invoke();
    }

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

        InventoryEvents.HotbarChanged?.Invoke();
    }

    // Helpers
    public bool ValidHotbarIndex(int i) =>
        i >= 0 && i < slots.Count;

    public bool ValidInventoryIndex(Inventory inv, int i) =>
        inv != null && i >= 0 && i < inv.slots.Count;

    public HotbarSaveData ToSaveData()
    {
        var data = new HotbarSaveData();

        foreach (var slot in slots)
        {
            data.itemIds.Add(new HotbarSlotSaveData
            {
                itemId = slot.item != null
                    ? slot.item.Id
                    : null
            });
        }

        return data;
    }

    public void LoadFromSaveData(HotbarSaveData data, Inventory inventory)
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].Clear();

        for (int i = 0; i < data.itemIds.Count && i < slots.Count; i++)
        {
            var itemId = data.itemIds[i].itemId;
            if (string.IsNullOrEmpty(itemId)) continue;

            var targetItem = ItemDatabase.Instance.Get(itemId);
            if (targetItem == null) continue;

            for (int invIndex = 0; invIndex < inventory.SlotCount; invIndex++)
            {
                var invSlot = inventory.GetSlot(invIndex);

                if (invSlot.item == targetItem)
                {
                    Assign(i, inventory, invIndex);
                    break;
                }
            }
        }

        InventoryEvents.HotbarChanged?.Invoke();
    }
}