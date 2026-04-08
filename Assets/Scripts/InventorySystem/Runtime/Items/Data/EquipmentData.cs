using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the data for an equipment item, including its slot and stat modifiers.
/// </summary>
[CreateAssetMenu(menuName = "Inventory/Equipment")]
public class EquipmentData : ItemData
{
    [Header("Equipment")]
    [SerializeField] private EquipmentSlot equipSlot;

    [Header("Stat Modifiers")]
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    public EquipmentSlot EquipSlot => equipSlot;
    public IReadOnlyList<StatModifier> Modifiers => modifiers;
}