using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour, IEquippedItemLookup
{
    [SerializeField] Inventory inventory;

    private Dictionary<EquipmentSlot, EquipmentData> equipped
        = new Dictionary<EquipmentSlot, EquipmentData>();

    private Dictionary<EquipmentSlot, int> equippedFromInventorySlotIndex
        = new Dictionary<EquipmentSlot, int>();

    private Dictionary<EquipmentSlot, InventorySlot> equippedSourceInventorySlot
        = new Dictionary<EquipmentSlot, InventorySlot>();

    private Dictionary<EquipmentData, List<StatModifier>> runtimeModifiers
        = new Dictionary<EquipmentData, List<StatModifier>>();

    public void BindInventory(Inventory inv) => inventory = inv;

    void OnEnable()
    {
        InventoryEvents.EquipRequested += OnEquipRequested;
        InventoryEvents.UnequipRequested += Unequip;
        InventoryEvents.InventorySlotsSwapped += OnInventorySlotsSwapped;
        InventoryEvents.InventoryChanged += OnInventoryLayoutMaybeChanged;
    }

    void OnDisable()
    {
        InventoryEvents.EquipRequested -= OnEquipRequested;
        InventoryEvents.UnequipRequested -= Unequip;
        InventoryEvents.InventorySlotsSwapped -= OnInventorySlotsSwapped;
        InventoryEvents.InventoryChanged -= OnInventoryLayoutMaybeChanged;
    }

    public EquipmentData GetEquipped(EquipmentSlot slot)
    {
        equipped.TryGetValue(slot, out var item);
        return item;
    }

    public bool IsInventorySlotSourceOfEquippedItem(int inventorySlotIndex, EquipmentData item)
    {
        if (item == null || inventory == null || inventorySlotIndex < 0 || !inventory.Valid(inventorySlotIndex))
            return false;

        if (!equipped.TryGetValue(item.equipSlot, out var equippedItem) || equippedItem != item)
            return false;

        if (!equippedSourceInventorySlot.TryGetValue(item.equipSlot, out var srcCell) || srcCell == null)
            return false;

        return ReferenceEquals(inventory.GetSlot(inventorySlotIndex), srcCell);
    }

    void OnEquipRequested(EquipmentData item, int sourceSlotIndex)
    {
        if (item == null) return;

        if (inventory == null)
        {
            Debug.LogWarning("Inventory is not bound.");
            return;
        }

        if (sourceSlotIndex < 0 || !inventory.Valid(sourceSlotIndex))
        {
            Debug.LogWarning("Invalid equipment source slot.");
            return;
        }

        var invSlot = inventory.GetSlot(sourceSlotIndex);
        if (invSlot?.item != item)
        {
            Debug.LogWarning("Source slot does not match the equipment item.");
            return;
        }

        var equipSlot = item.equipSlot;

        if (equipped.TryGetValue(equipSlot, out var oldItem) && oldItem != null && oldItem != item)
            RemoveEquippedSilent(equipSlot);

        ApplyEquippedState(item, sourceSlotIndex);
    }

    void Unequip(EquipmentSlot slot)
    {
        if (!equipped.TryGetValue(slot, out var item))
            return;

        RemoveEquippedSilent(slot);

        Debug.Log($"Unequipped {item.itemName}");
    }

    void ApplyEquippedState(EquipmentData item, int sourceInventorySlotIndex)
    {
        if (item == null) return;

        var equipSlot = item.equipSlot;

        if (equipped.TryGetValue(equipSlot, out var existing) && existing == item)
        {
            SetEquippedSourceCell(equipSlot, sourceInventorySlotIndex);
            InventoryEvents.EquipmentChanged?.Invoke();
            return;
        }

        equipped[equipSlot] = item;
        SetEquippedSourceCell(equipSlot, sourceInventorySlotIndex);

        var copies = new List<StatModifier>();
        foreach (var mod in item.modifiers)
            copies.Add(mod.Clone());

        runtimeModifiers[item] = copies;

        InventoryEvents.OnEquipped?.Invoke(item, runtimeModifiers[item]);
        InventoryEvents.EquipmentChanged?.Invoke();

        Debug.Log($"Equipped {item.itemName}");
    }

    void SetEquippedSourceCell(EquipmentSlot equipSlot, int sourceInventorySlotIndex)
    {
        if (inventory == null || sourceInventorySlotIndex < 0 || !inventory.Valid(sourceInventorySlotIndex))
        {
            equippedFromInventorySlotIndex.Remove(equipSlot);
            equippedSourceInventorySlot.Remove(equipSlot);
            return;
        }

        equippedFromInventorySlotIndex[equipSlot] = sourceInventorySlotIndex;
        equippedSourceInventorySlot[equipSlot] = inventory.GetSlot(sourceInventorySlotIndex);
    }

    void RemoveEquippedSilent(EquipmentSlot slot)
    {
        if (!equipped.TryGetValue(slot, out var item))
            return;

        equipped.Remove(slot);
        equippedFromInventorySlotIndex.Remove(slot);
        equippedSourceInventorySlot.Remove(slot);

        if (runtimeModifiers.TryGetValue(item, out var mods))
        {
            runtimeModifiers.Remove(item);
            InventoryEvents.OnUnequipped?.Invoke(item, mods);
        }

        InventoryEvents.EquipmentChanged?.Invoke();
    }

    void OnInventorySlotsSwapped(int a, int b)
    {
        if (inventory == null)
            return;

        foreach (var equipSlot in new List<EquipmentSlot>(equippedFromInventorySlotIndex.Keys))
        {
            if (!equippedFromInventorySlotIndex.TryGetValue(equipSlot, out var idx))
                continue;

            if (idx == a)
                SetEquippedSourceCell(equipSlot, b);
            else if (idx == b)
                SetEquippedSourceCell(equipSlot, a);
        }

        InventoryEvents.EquipmentChanged?.Invoke();
    }

    void OnInventoryLayoutMaybeChanged()
    {
        if (inventory == null)
            return;

        bool dirty = false;

        foreach (var equipSlot in new List<EquipmentSlot>(equipped.Keys))
        {
            if (!equippedSourceInventorySlot.TryGetValue(equipSlot, out var cell) || cell == null)
                continue;

            int newIdx = inventory.IndexOfSlot(cell);
            if (newIdx < 0)
                continue;

            if (!equipped.TryGetValue(equipSlot, out var eqItem) || eqItem == null)
                continue;

            if (cell.item != eqItem)
                continue;

            if (!equippedFromInventorySlotIndex.TryGetValue(equipSlot, out var oldIdx) || oldIdx != newIdx)
            {
                equippedFromInventorySlotIndex[equipSlot] = newIdx;
                dirty = true;
            }
        }

        if (dirty)
            InventoryEvents.EquipmentChanged?.Invoke();
    }

    public void UnequipAll()
    {
        var slots = new List<EquipmentSlot>(equipped.Keys);

        foreach (var slot in slots)
            RemoveEquippedSilent(slot);
    }

    public EquipmentSaveData ToSaveData()
    {
        var data = new EquipmentSaveData();

        foreach (var kv in equipped)
        {
            int src = equippedFromInventorySlotIndex.TryGetValue(kv.Key, out var si) ? si : -1;
            data.slots.Add(new EquipmentSlotSaveData
            {
                slotName = kv.Key.ToString(),
                itemId = kv.Value != null ? kv.Value.Id : null,
                sourceInventorySlotIndex = src
            });
        }

        return data;
    }

    public void LoadFromSaveData(EquipmentSaveData data)
    {
        LoadFromSaveData(data, ItemDatabase.Instance);
    }

    /// <summary>
    /// Restores equipped items from save.
    /// </summary>
    public void LoadFromSaveData(EquipmentSaveData data, IItemDatabase itemDatabase)
    {
        // UnequipAll first clears current state (fires OnUnequipped -> Removes modifiers).
        // Then we Equip each saved item (fires OnEquipped -> Adds modifiers).
        // Order ensures no double-application of modifiers.
        UnequipAll();

        foreach (var slot in data.slots)
        {
            if (string.IsNullOrEmpty(slot.itemId))
                continue;

            var item = itemDatabase?.Get(slot.itemId);

            if (item is EquipmentData eq)
                ApplyEquippedState(eq, slot.sourceInventorySlotIndex);
        }

        ResolveEquippedSourcesFromInventory();
    }

    public void ResolveEquippedSourcesFromInventory()
    {
        if (inventory == null)
            return;

        foreach (var equipSlot in new List<EquipmentSlot>(equipped.Keys))
        {
            var item = equipped[equipSlot];
            if (item == null)
                continue;

            if (equippedFromInventorySlotIndex.TryGetValue(equipSlot, out var src)
                && src >= 0
                && inventory.Valid(src)
                && inventory.GetSlot(src)?.item == item)
            {
                equippedSourceInventorySlot[equipSlot] = inventory.GetSlot(src);
                continue;
            }

            int found = FindFirstInventorySlotWithItem(inventory, item);
            if (found >= 0)
                SetEquippedSourceCell(equipSlot, found);
        }

        InventoryEvents.EquipmentChanged?.Invoke();
    }

    static int FindFirstInventorySlotWithItem(Inventory inv, EquipmentData item)
    {
        if (inv == null || item == null)
            return -1;

        for (int i = 0; i < inv.SlotCount; i++)
        {
            var s = inv.GetSlot(i);
            if (s?.item == item && s.count > 0)
                return i;
        }

        return -1;
    }
}