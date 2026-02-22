using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    private Dictionary<EquipmentSlot, EquipmentData> equipped
        = new Dictionary<EquipmentSlot, EquipmentData>();

    private Dictionary<EquipmentData, List<StatModifier>> runtimeModifiers
        = new Dictionary<EquipmentData, List<StatModifier>>();

    void OnEnable()
    {
        InventoryEvents.EquipRequested += Equip;
        InventoryEvents.UnequipRequested += Unequip;
    }

    void OnDisable()
    {
        InventoryEvents.EquipRequested -= Equip;
        InventoryEvents.UnequipRequested -= Unequip;
    }

    // ---------- Public API ----------
    public EquipmentData GetEquipped(EquipmentSlot slot)
    {
        equipped.TryGetValue(slot, out var item);
        return item;
    }

    public bool IsEquipped(EquipmentData item)
    {
        if (item == null)
            return false;
        
        if (!equipped.TryGetValue(item.equipSlot, out var equippedItem))
            return false;

        return equippedItem == item;
    }

    // ---------- API ----------

    void Equip(EquipmentData item)
    {
        if (item == null) return;

        var slot = item.equipSlot;

        // If the slot is already equipped, unequip first
        if (equipped.TryGetValue(slot, out var oldItem))
        {
            Unequip(slot);
        }

        equipped[slot] = item;

        // Create runtime modifier copies
        var copies = new List<StatModifier>();
        foreach (var mod in item.modifiers)
        {
            copies.Add(mod.Clone());
        }

        runtimeModifiers[item] = copies;

        InventoryEvents.OnEquipped?.Invoke(item, runtimeModifiers[item]);
        InventoryEvents.EquipmentChanged?.Invoke();

        Debug.Log($"Equipped {item.itemName}");
    }

    //void Unequip(EquipmentSlot slot)
    //{
    //    if (!equipped.TryGetValue(slot, out var item))
    //        return;

        
    //    if (runtimeModifiers.TryGetValue(item, out var mods))
    //    {
    //        GameEvents.OnUnequipped?.Invoke(item, mods);
    //        runtimeModifiers.Remove(item);
    //        GameEvents.EquipmentChanged?.Invoke();
    //    }

    //    equipped.Remove(slot);

    //    Debug.Log($"[Unequip] equipped.Count = {equipped.Count}");

    //    Debug.Log($"Unequipped {item.itemName}");
    //}
    void Unequip(EquipmentSlot slot)
    {
        if (!equipped.TryGetValue(slot, out var item))
            return;

        // Remove the equipped status
        equipped.Remove(slot);

        // Unequip event with a runtime modifier copy
        if (runtimeModifiers.TryGetValue(item, out var mods))
        {
            runtimeModifiers.Remove(item);
            InventoryEvents.OnUnequipped?.Invoke(item, mods);
        }

        // Inform the system to refresh
        InventoryEvents.EquipmentChanged?.Invoke();

        Debug.Log($"[Unequip] equipped.Count = {equipped.Count}");
        Debug.Log($"Unequipped {item.itemName}");
    }

    public void UnequipAll()
    {
        var slots = new List<EquipmentSlot>(equipped.Keys);

        foreach (var slot in slots)
        {
            Unequip(slot);
        }
    }

    public EquipmentSaveData ToSaveData()
    {
        var data = new EquipmentSaveData();

        foreach (var kv in equipped)
        {
            data.slots.Add(new EquipmentSlotSaveData
            {
                slotName = kv.Key.ToString(),
                itemId = kv.Value != null ? kv.Value.Id : null
            });
        }

        return data;
    }

    public void LoadFromSaveData(EquipmentSaveData data, Inventory inventory)
    {
        UnequipAll();

        foreach (var slot in data.slots)
        {
            if (string.IsNullOrEmpty(slot.itemId))
                continue;

            var item = ItemDatabase.Instance.Get(slot.itemId);

            if (item is EquipmentData eq)
            {
                Equip(eq);
            }
        }
    }
}