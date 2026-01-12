using UnityEngine;

[System.Serializable]
public class StatModifier
{
    public StatType statType;
    public int value;
    public ModifierType modifierType;

    public StatModifier(StatType type, int value, ModifierType modType)
    {
        statType = type;
        this.value = value;
        modifierType = modType;
    }
}

public enum ModifierType
{
    Flat,       // +10
    Percent     // +10%
}