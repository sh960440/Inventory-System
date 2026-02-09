[System.Serializable]
public class SaveData
{
    public int version = 1;

    public InventorySaveData inventory;
    public HotbarSaveData hotbar;
    public EquipmentSaveData equipment;
}