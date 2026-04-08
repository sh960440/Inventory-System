/// <summary>
/// Represents a single equipped slot in the save data, including the slot type and assigned item.
/// </summary>
[System.Serializable]
public class EquipmentSlotSaveData
{
    public string slotName;
    public string itemId;

    /// <summary>
    /// The inventory slot index the equipped item originated from, or -1 if unknown.
    /// </summary>
    public int sourceInventorySlotIndex = -1;
}