/// <summary>
/// Represents a single inventory slot in the save data, including item ID and stack count.
/// </summary>
[System.Serializable]
public class InventorySlotSaveData
{
    public string itemId;
    public int count;
}