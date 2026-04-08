using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the data for a consumable item, including its instant and timed stat effects.
/// </summary>
[CreateAssetMenu(menuName = "Inventory/Consumable")]
public class ConsumableData : ItemData
{
    [Header("Effects")]
    [SerializeField] private List<StatModifier> instantModifiers = new List<StatModifier>();
    [SerializeField] private List<StatModifier> durationModifiers = new List<StatModifier>();
    [SerializeField] private float duration;

    public IReadOnlyList<StatModifier> InstantModifiers => instantModifiers;
    public IReadOnlyList<StatModifier> DurationModifiers => durationModifiers;
    public float Duration => duration;
}