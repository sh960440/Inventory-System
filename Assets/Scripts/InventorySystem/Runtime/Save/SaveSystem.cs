using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public Inventory inventory;
    public Hotbar hotbar;
    public Equipment equipment;
    [SerializeField] private MonoBehaviour itemDatabaseProvider;

    string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    public void Save()
    {
        Debug.Log("Save started");

        var data = new SaveData
        {
            inventory = inventory.ToSaveData(),
            hotbar = hotbar.ToSaveData(),
            equipment = equipment.ToSaveData()
        };

        File.WriteAllText(
            SavePath,
                JsonUtility.ToJson(data, true)
        );

        Debug.Log($"Saving to {SavePath}");
    }

    public void LoadV1(SaveData data)
    {
        var db = itemDatabaseProvider as IItemDatabase;
        if (db == null)
            db = ItemDatabase.Instance;

        inventory.LoadFromSaveData(data.inventory, db);
        hotbar.LoadFromSaveData(data.hotbar, inventory, db);
        equipment.LoadFromSaveData(data.equipment, inventory, db);
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Save file not found");
            return;
        }

        var json = File.ReadAllText(SavePath);

        SaveData data = null;

        try
        {
            data = JsonUtility.FromJson<SaveData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to parse save file: {e}");
            return;
        }

        if (data == null)
        {
            Debug.LogError("SaveData is null after deserialization.");
            return;
        }

        if (data.inventory == null)
        {
            Debug.LogError("Inventory data missing.");
            return;
        }

        switch (data.version)
        {
            case 1:
                LoadV1(data);
                break;
            default:
                Debug.LogWarning($"Unsupported save version: {data.version}");
                break;
        }
    }
}
