using System.Collections.Generic;

/// <summary>
/// Represents the serialized state of the hotbar, stored as an ordered list of slot entries.
/// </summary>
[System.Serializable]
public class HotbarSaveData
{
    public List<HotbarSlotSaveData> itemIds = new();
}