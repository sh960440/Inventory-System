using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public Inventory inventory;
    public Hotbar hotbar;
    public EquipmentManager equipment;

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

    public void Load()
    {
        Debug.Log("Load called");

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

        // Check version compatibility
        // TODO: Create LoadV1, LoadV2, etc. for different versions
        if (data.version != 1)
        {
            Debug.LogWarning($"Unsupported save version: {data.version}");
            return;
        }

        inventory.LoadFromSaveData(data.inventory, inventory);
        hotbar.LoadFromSaveData(data.hotbar, inventory);
        equipment.LoadFromSaveData(data.equipment, inventory);
    }
}
