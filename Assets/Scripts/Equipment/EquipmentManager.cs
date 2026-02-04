using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private Dictionary<EquipmentSlot, EquipmentData> equipped
        = new Dictionary<EquipmentSlot, EquipmentData>();

    private Dictionary<EquipmentData, List<StatModifier>> runtimeModifiers
        = new Dictionary<EquipmentData, List<StatModifier>>();

    void OnEnable()
    {
        GameEvents.OnEquipRequested += Equip;
        GameEvents.OnUnequipRequested += Unequip;
    }

    void OnDisable()
    {
        GameEvents.OnEquipRequested -= Equip;
        GameEvents.OnUnequipRequested -= Unequip;
    }

    // ---------- Public API ----------
    public EquipmentData GetEquipped(EquipmentSlot slot)
    {
        equipped.TryGetValue(slot, out var item);
        return item;
    }

    public bool IsEquipped(EquipmentData item)
    {
        return equipped.ContainsValue(item);
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

        GameEvents.OnEquipped?.Invoke(item, runtimeModifiers[item]);
        GameEvents.OnEquipmentChanged?.Invoke();

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
    //        GameEvents.OnEquipmentChanged?.Invoke();
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
            GameEvents.OnUnequipped?.Invoke(item, mods);
        }

        // Inform the system to refresh
        GameEvents.OnEquipmentChanged?.Invoke();

        Debug.Log($"[Unequip] equipped.Count = {equipped.Count}");
        Debug.Log($"Unequipped {item.itemName}");
    }
}