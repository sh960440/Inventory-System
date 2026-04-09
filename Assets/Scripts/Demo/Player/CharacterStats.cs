using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private int maxHP = 200;
    [SerializeField] private int currentHP = 100;
    [SerializeField] private int baseAttack = 10;
    [SerializeField] private int baseDefense = 5;
    [SerializeField] private float baseMoveSpeed = 5f;

    private readonly List<StatModifier> _modifiers = new List<StatModifier>();

    public static event Action OnStatsChanged;
    public static event Action<int> OnHealed;
    public static event Action<int> OnHPChanged;

    public int CurrentHP => currentHP;
    public int GetMaxHP() => Mathf.Max(1, maxHP);

    private void Awake()
    {
        ClampCurrentToMaxHPField();
    }

    private void OnEnable()
    {
        InventoryEvents.OnEquipped += OnEquipped;
        InventoryEvents.OnUnequipped += OnUnequipped;
    }

    private void OnDisable()
    {
        InventoryEvents.OnEquipped -= OnEquipped;
        InventoryEvents.OnUnequipped -= OnUnequipped;
    }

    public void AddModifier(StatModifier mod)
    {
        _modifiers.Add(mod);
        int hpBefore = currentHP;
        ApplyHpModifierToCurrent(mod, +1);
        ClampCurrentToMaxHPField();
        OnStatsChanged?.Invoke();
        if (currentHP != hpBefore)
            OnHPChanged?.Invoke(currentHP);
    }

    public void RemoveModifier(StatModifier mod)
    {
        if (!_modifiers.Remove(mod))
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
        if (type == StatType.Health)
            return maxHP;

        float baseValue = GetBaseValue(type);
        float flatBonus = 0f;
        float percentBonus = 0f;

        foreach (var mod in _modifiers)
        {
            if (mod.StatType != type)
                continue;

            if (mod.ModifierType == ModifierType.Flat)
                flatBonus += mod.Value;
            else
                percentBonus += mod.Value;
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
    private void EditorDemoTakeDamage() => TakeDamage(50);
#endif

    private void OnEquipped(EquipmentData item, List<StatModifier> mods)
    {
        foreach (var mod in mods)
            AddModifier(mod);
    }

    private void OnUnequipped(EquipmentData item, List<StatModifier> mods)
    {
        foreach (var mod in mods)
            RemoveModifier(mod);
    }

    private void ApplyHpModifierToCurrent(StatModifier mod, int sign)
    {
        if (mod.StatType != StatType.Health)
            return;

        int delta;
        if (mod.ModifierType == ModifierType.Flat)
            delta = Mathf.RoundToInt(mod.Value) * sign;
        else
            delta = Mathf.RoundToInt(maxHP * (mod.Value / 100f)) * sign;

        currentHP += delta;
    }

    private void ClampCurrentToMaxHPField()
    {
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    private float GetBaseValue(StatType type)
    {
        switch (type)
        {
            case StatType.Health: return maxHP;
            case StatType.Attack: return baseAttack;
            case StatType.Defense: return baseDefense;
            case StatType.MoveSpeed: return baseMoveSpeed;
            default: return 0f;
        }
    }
}