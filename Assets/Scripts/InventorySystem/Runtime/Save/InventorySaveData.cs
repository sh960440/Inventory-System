using System.Collections.Generic;

/// <summary>
/// Represents the serialized state of the inventory, storing one entry per slot.
/// </summary>
[System.Serializable]
public class InventorySaveData
{
    public List<InventorySlotSaveData> slots = new();
}