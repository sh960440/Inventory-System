using System.Collections.Generic;

[System.Serializable]
public class EquipmentSaveData
{
    // key = equip slot name (string or enum.ToString)
    // value = itemId
    public Dictionary<string, string> equippedItems = new();
}