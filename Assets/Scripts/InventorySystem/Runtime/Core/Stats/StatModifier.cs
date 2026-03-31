using UnityEngine;

/// <summary>
/// A stat modifier used to adjust attributes.
/// </summary>
[System.Serializable]
public sealed class StatModifier
{
    [SerializeField] private StatType statType;
    [SerializeField] private int value;
    [SerializeField] private ModifierType modifierType;

    public StatType StatType => statType;
    public int Value => value;
    public ModifierType ModifierType => modifierType;

    public StatModifier(StatType type, int value, ModifierType modifierType)
    {
        this.statType = type;
        this.value = value;
        this.modifierType = modifierType;
    }

    public StatModifier Clone()
    {
        return new StatModifier(statType, value, modifierType);
    }
}