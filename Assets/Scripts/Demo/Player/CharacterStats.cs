using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int maxHP = 200;
    public int currentHP = 100;
    public int baseAttack = 10;
    public int baseDefense = 5;
    public float baseMoveSpeed = 5f;

    private List<StatModifier> modifiers = new List<StatModifier>();

    public static event Action OnStatsChanged;
    public static event Action<int> OnHealed;
    public static event Action<int> OnHPChanged;

    public int CurrentHP => currentHP;
    public int GetMaxHP() => Mathf.Max(1, maxHP);

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

    void Awake()
    {
        ClampCurrentToMaxHPField();
    }

    void Start()
    {
        ClampCurrentToMaxHPField();
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

    public void AddModifier(StatModifier mod)
    {
        modifiers.Add(mod);
        int hpBefore = currentHP;
        ApplyHpModifierToCurrent(mod, +1);
        ClampCurrentToMaxHPField();
        OnStatsChanged?.Invoke();
        if (currentHP != hpBefore)
            OnHPChanged?.Invoke(currentHP);
    }

    public void RemoveModifier(StatModifier mod)
    {
        if (!modifiers.Remove(mod))
            return;

        int hpBefore = currentHP;
        ApplyHpModifierToCurrent(mod, -1);
        ClampCurrentToMaxHPField();
        OnStatsChanged?.Invoke();
        if (currentHP != hpBefore)
            OnHPChanged?.Invoke(currentHP);
    }

    public float GetFinalValue(StatType type)
    {
        if (type == StatType.HP)
            return maxHP;

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

        return (baseValue + flatBonus) * (1 + percentBonus / 100f);
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
            return;

        int oldHP = currentHP;
        currentHP = Mathf.Min(currentHP + amount, maxHP);

        int actualHeal = currentHP - oldHP;

        if (actualHeal > 0)
        {
            OnHealed?.Invoke(actualHeal);
            OnHPChanged?.Invoke(currentHP);
        }
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0)
            return;

        int oldHP = currentHP;
        currentHP = Mathf.Max(1, currentHP - amount);

        if (currentHP != oldHP)
        {
            OnHPChanged?.Invoke(currentHP);
            OnStatsChanged?.Invoke();
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Demo/Take 50 damage")]
    void EditorDemoTakeDamage() => TakeDamage(50);
#endif

    void ApplyHpModifierToCurrent(StatModifier mod, int sign)
    {
        if (mod.statType != StatType.HP)
            return;

        int delta;
        if (mod.modifierType == ModifierType.Flat)
            delta = Mathf.RoundToInt(mod.value) * sign;
        else
            delta = Mathf.RoundToInt(maxHP * (mod.value / 100f)) * sign;

        currentHP += delta;
    }

    void ClampCurrentToMaxHPField()
    {
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    float GetBaseValue(StatType type)
    {
        switch (type)
        {
            case StatType.HP: return maxHP;
            case StatType.Attack: return baseAttack;
            case StatType.Defense: return baseDefense;
            case StatType.MoveSpeed: return baseMoveSpeed;
            default: return 0;
        }
    }
}