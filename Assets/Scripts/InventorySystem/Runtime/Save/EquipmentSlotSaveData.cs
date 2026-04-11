/// <summary>
/// Represents a single equipped slot in the save data, including the slot type and assigned item.
/// </summary>
[System.Serializable]
public class EquipmentSlotSaveData
{
    public string slotName;
    public string itemId;
    public int inventorySlotIndex = -1;
}