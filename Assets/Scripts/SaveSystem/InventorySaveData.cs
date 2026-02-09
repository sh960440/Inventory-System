using System.Collections.Generic;

[System.Serializable]
public class InventorySaveData
{
    public List<InventorySlotSaveData> slots = new();
}