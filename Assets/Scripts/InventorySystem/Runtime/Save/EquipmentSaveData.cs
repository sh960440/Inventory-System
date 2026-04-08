using System.Collections.Generic;

/// <summary>
/// Represents the serialized state of equipped items, stored per equipment slot.
/// </summary>
[System.Serializable]
public class EquipmentSaveData
{
    public List<EquipmentSlotSaveData> slots = new();
}