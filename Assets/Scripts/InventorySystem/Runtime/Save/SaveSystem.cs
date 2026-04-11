using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Saves and loads game data as JSON using the persistent data path.
/// </summary>
public class SaveSystem : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Hotbar hotbar;
    [SerializeField] private Equipment equipment;

    [Header("Layout validation")]
    [SerializeField] private ItemSystemConfiguration itemSystemConfiguration;

    [Header("Database")]
    [SerializeField] private MonoBehaviour itemDatabaseProvider;

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    /// <summary>
    /// Writes inventory, hotbar, and equipment snapshots to the save file.
    /// </summary>
    public void Save()
    {
        if (inventory == null && hotbar == null && equipment == null)
            return;

        var data = new SaveData
        {
            version = 1,
            inventory = inventory != null ? inventory.ToSaveData() : new InventorySaveData()
        };

        if (hotbar != null)
            data.hotbar = hotbar.ToSaveData();

        if (equipment != null)
            data.equipment = equipment.ToSaveData();

        try
        {
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        }
        catch (Exception e)
        {
            Debug.LogError("SaveSystem: failed to write file");
            return;
        }

        Debug.Log($"Saved to {SavePath}");
    }

    /// <summary>
    /// Reads the save file if it exists and applies it.
    /// </summary>
    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Save file not found.");
            return;
        }

        string json = File.ReadAllText(SavePath);

        SaveData data;

        try
        {
            data = JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e)
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

    private void LoadV1(SaveData data)
    {
        if (inventory == null)
            return;

        var db = itemDatabaseProvider as IItemDatabase;
        if (db == null)
            db = ItemDatabase.Instance;

        int expectedInventorySlots = itemSystemConfiguration != null
            ? itemSystemConfiguration.InventorySlotCount
            : inventory.SlotCount;

        if (!inventory.TryLoadFromSaveData(data.inventory, db, expectedInventorySlots))
        {
            Debug.LogWarning("Full save load aborted: inventory layout does not match save file.");
            return;
        }

        if (hotbar != null)
            hotbar.LoadFromSaveData(data.hotbar ?? new HotbarSaveData(), inventory, db);

        if (equipment != null)
            equipment.LoadFromSaveData(data.equipment, db);
    }
}