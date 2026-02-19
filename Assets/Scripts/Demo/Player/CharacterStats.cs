using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int baseMaxHP = 100;
    public int baseAttack = 10;
    public int baseDefense = 5;
    public float baseMoveSpeed = 5f;

    private List<StatModifier> modifiers = new List<StatModifier>();

    void OnEnable()
    {
        InventoryEvents.OnEquipped += OnEquipped;
        InventoryEvents.OnUnequipped += OnUnequipped;
    }

    void OnDisable()
    {
        InventoryEvents.OnEquipped -= OnEquipped;
        InventoryEvents.OnUnequipped -= OnUnequipped;
    }

    void OnEquipped(EquipmentData item, List<StatModifier> mods)
    {
        foreach (var mod in mods)
            AddModifier(mod);
    }

    void OnUnequipped(EquipmentData item, List<StatModifier> mods)
    {
        foreach (var mod in mods)
            RemoveModifier(mod);
    }

    // ---------- Public API ----------

    public void AddModifier(StatModifier mod)
    {
        modifiers.Add(mod);
    }

    public void RemoveModifier(StatModifier mod)
    {
        modifiers.Remove(mod);
    }

    public float GetFinalValue(StatType type)
    {
        float baseValue = GetBaseValue(type);
        float flatBonus = 0f;
        float percentBonus = 0f;

        foreach (var mod in modifiers)
        {
            if (mod.statType != type) continue;

            if (mod.modifierType == ModifierType.Flat)
                flatBonus += mod.value;
            else
                percentBonus += mod.value;
        }

        float finalValue = (baseValue + flatBonus) * (1 + percentBonus / 100f);
        return finalValue;
    }

    // ---------- Internal ----------

    float GetBaseValue(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHP: return baseMaxHP;
            case StatType.Attack: return baseAttack;
            case StatType.Defense: return baseDefense;
            case StatType.MoveSpeed: return baseMoveSpeed;
            default: return 0;
        }
    }
}