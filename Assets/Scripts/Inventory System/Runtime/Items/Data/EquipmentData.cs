using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Equipment")]
public class EquipmentData : ItemData
{
    public EquipmentSlot equipSlot;

    [Header("Stat Modifiers")]
    public List<StatModifier> modifiers = new List<StatModifier>();
}