using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Consumable")]
public class ConsumableData : ItemData
{
    public List<StatModifier> instantModifiers;
    public List<StatModifier> durationModifiers;
    public float duration;
}