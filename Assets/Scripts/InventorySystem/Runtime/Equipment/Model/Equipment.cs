using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Equipment state manager. Tracks equipped items and raises equipment events.
/// </summary>
public class Equipment : MonoBehaviour, IEquippedItemLookup
{
    [Header("References")]
    [SerializeField] private Inventory inventory;

    private readonly Dictionary<EquipmentSlot, EquipmentData> equipped = new Dictionary<EquipmentSlot, EquipmentData>();
    private readonly Dictionary<EquipmentSlot, int> equippedFromInventorySlotIndex = new Dictionary<EquipmentSlot, int>();
    // Keeps track of where each equipped item came from so highlights don’t break when sorting
    private readonly Dictionary<EquipmentSlot, InventorySlot> equippedSourceInventorySlot = new Dictionary<EquipmentSlot, InventorySlot>();
    private readonly Dictionary<EquipmentData, List<StatModifier>> runtimeModifiers = new Dictionary<EquipmentData, List<StatModifier>>();

    /// <summary>
    /// Sets the inventory used to check and track source _slots.
    /// </summary>
    public void BindInventory(Inventory inv) => inventory = inv;

    private void OnEnable()
    {
        InventoryEvents.EquipRequested += OnEquipRequested;
        InventoryEvents.UnequipRequested += Unequip;
        InventoryEvents.InventorySlotsSwapped += OnInventorySlotsSwapped;
        InventoryEvents.InventoryChanged += OnInventoryLayoutMaybeChanged;
    }

    private void OnDisable()
    {
        InventoryEvents.EquipRequested -= OnEquipRequested;
        InventoryEvents.UnequipRequested -= Unequip;
        InventoryEvents.InventorySlotsSwapped -= OnInventorySlotsSwapped;
        InventoryEvents.InventoryChanged -= OnInventoryLayoutMaybeChanged;
    }

    /// <summary>
    /// Returns the currently equipped item in the given equipment slot.
    /// </summary>
    public EquipmentData GetEquipped(EquipmentSlot slot)
    {
        equipped.TryGetValue(slot, out var item);
        return item;
    }

    /// <summary>
    /// Returns true when the given inventory index currently holds the source cell of this equipped item.
    /// </summary>
    public bool IsInventorySlotSourceOfEquippedItem(int inventorySlotIndex, EquipmentData item)
    {
        if (item == null || inventory == null || inventorySlotIndex < 0 || !inventory.Valid(inventorySlotIndex))
            return false;

        if (!equipped.TryGetValue(item.EquipSlot, out var equippedItem) || equippedItem != item)
            return false;

        if (!equippedSourceInventorySlot.TryGetValue(item.EquipSlot, out var srcCell) || srcCell == null)
            return false;

        return ReferenceEquals(inventory.GetSlot(inventorySlotIndex), srcCell);
    }

    /// <summary>
    /// Unequips all currently equipped items.
    /// </summary>
    public void UnequipAll()
    {
        var slots = new List<EquipmentSlot>(equipped.Keys);

        foreach (var slot in slots)
            RemoveEquippedSilent(slot);
    }

    /// <summary>
    /// Creates serializable equipment save data.
    /// </summary>
    public EquipmentSaveData ToSaveData()
    {
        var data = new EquipmentSaveData();

        foreach (var entry in equipped)
        {
            int sourceIndex = equippedFromInventorySlotIndex.TryGetValue(entry.Key, out var index) ? index : -1;
            data.slots.Add(new EquipmentSlotSaveData
            {
                slotName = entry.Key.ToString(),
                itemId = entry.Value != null ? entry.Value.Id : null,
                sourceInventorySlotIndex = sourceIndex
            });
        }

        return data;
    }

    /// <summary>
    /// Loads equipment save data.
    /// </summary>
    public void LoadFromSaveData(EquipmentSaveData data)
    {
        LoadFromSaveData(data, ItemDatabase.Instance);
    }

    /// <summary>
    /// Loads equipment save data.
    /// </summary>
    public void LoadFromSaveData(EquipmentSaveData data, IItemDatabase itemDatabase)
    {
        // UnequipAll first clears current state (fires OnUnequipped -> Removes modifiers).
        // Then Equip each saved item (fires OnEquipped -> Adds modifiers).
        // Order ensures no double-application of modifiers.
        UnequipAll();

        if (data == null || data.slots == null)
            return;

        foreach (var savedSlot in data.slots)
        {
            if (string.IsNullOrEmpty(savedSlot.itemId))
                continue;

            var item = itemDatabase?.Get(savedSlot.itemId);

            if (item is EquipmentData equipmentData)
                ApplyEquippedState(equipmentData, savedSlot.sourceInventorySlotIndex);
        }

        ResolveEquippedSourcesFromInventory();
    }

    /// <summary>
    /// Rebuilds equipped-source mapping by matching equipped items against current inventory _slots.
    /// </summary>
    public void ResolveEquippedSourcesFromInventory()
    {
        if (inventory == null)
            return;

        foreach (var equipSlot in new List<EquipmentSlot>(equipped.Keys))
        {
            var item = equipped[equipSlot];
            if (item == null)
                continue;

            if (equippedFromInventorySlotIndex.TryGetValue(equipSlot, out var sourceIndex)
                && sourceIndex >= 0
                && inventory.Valid(sourceIndex)
                && inventory.GetSlot(sourceIndex)?.Item == item)
            {
                equippedSourceInventorySlot[equipSlot] = inventory.GetSlot(sourceIndex);
                continue;
            }

            int foundIndex = FindFirstInventorySlotWithItem(inventory, item);
            if (foundIndex >= 0)
                SetEquippedSourceCell(equipSlot, foundIndex);
        }

        InventoryEvents.EquipmentChanged?.Invoke();
    }

    private void OnEquipRequested(EquipmentData item, int sourceSlotIndex)
    {
        if (item == null) 
            return;

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
        if (invSlot?.Item != item)
        {
            Debug.LogWarning("Source slot does not match the equipment item.");
            return;
        }

        var equipSlot = item.EquipSlot;

        if (equipped.TryGetValue(equipSlot, out var oldItem) && oldItem != null && oldItem != item)
            RemoveEquippedSilent(equipSlot);

        ApplyEquippedState(item, sourceSlotIndex);
    }

    private void Unequip(EquipmentSlot slot)
    {
        if (!equipped.TryGetValue(slot, out var item))
            return;

        RemoveEquippedSilent(slot);

        Debug.Log($"Unequipped {item.ItemName}");
    }

    private void ApplyEquippedState(EquipmentData item, int sourceInventorySlotIndex)
    {
        if (item == null)
            return;

        var equipSlot = item.EquipSlot;

        if (equipped.TryGetValue(equipSlot, out var existing) && existing == item)
        {
            SetEquippedSourceCell(equipSlot, sourceInventorySlotIndex);
            InventoryEvents.EquipmentChanged?.Invoke();
            return;
        }

        equipped[equipSlot] = item;
        SetEquippedSourceCell(equipSlot, sourceInventorySlotIndex);

        var copies = new List<StatModifier>();
        foreach (var mod in item.Modifiers)
            copies.Add(mod.Clone());

        runtimeModifiers[item] = copies;

        InventoryEvents.OnEquipped?.Invoke(item, runtimeModifiers[item]);
        InventoryEvents.EquipmentChanged?.Invoke();

        Debug.Log($"Equipped {item.ItemName}");
    }

    private void SetEquippedSourceCell(EquipmentSlot equipSlot, int sourceInventorySlotIndex)
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

    private void RemoveEquippedSilent(EquipmentSlot slot)
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

    private void OnInventorySlotsSwapped(int a, int b)
    {
        if (inventory == null)
            return;

        bool changed = false;
        foreach (var equipSlot in new List<EquipmentSlot>(equippedFromInventorySlotIndex.Keys))
        {
            if (!equippedFromInventorySlotIndex.TryGetValue(equipSlot, out var idx))
                continue;

            if (idx == a)
            {
                SetEquippedSourceCell(equipSlot, b);
                changed = true;
            }
            else if (idx == b)
            {
                SetEquippedSourceCell(equipSlot, a);
                changed = true;
            }
        }

        if (changed)
            InventoryEvents.EquipmentChanged?.Invoke();
    }

    private void OnInventoryLayoutMaybeChanged()
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

            if (cell.Item != eqItem)
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

    private static int FindFirstInventorySlotWithItem(Inventory inv, EquipmentData item)
    {
        if (inv == null || item == null)
            return -1;

        for (int i = 0; i < inv.SlotCount; i++)
        {
            var s = inv.GetSlot(i);
            if (s?.Item == item && s.Count > 0)
                return i;
        }

        return -1;
    }
}