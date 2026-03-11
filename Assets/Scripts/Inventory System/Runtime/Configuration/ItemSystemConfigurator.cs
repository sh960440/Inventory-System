using UnityEngine;

public class ItemSystemConfigurator : MonoBehaviour
{
    [SerializeField] private ItemSystemConfiguration config;

    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Equipment equipment;
    [SerializeField] private Hotbar hotbar;
    [SerializeField] private InventoryUIController inventoryUI;
    [SerializeField] private EquipmentUIController equipmentUI;
    [SerializeField] private HotbarUIController hotbarUI;

    private void Awake()
    {
        ApplyConfig();
    }

    private void ApplyConfig()
    {
        if (inventory != null)
            inventory.ApplyConfig(config, equipment);

        if (inventoryUI != null)
            inventoryUI.ApplyConfig(config, equipment);

        //if (equipment != null)
        //    equipment.ApplyConfig(config);

        //if (equipmentUI != null)
        //    equipmentUI.ApplyConfig(config);

        if (hotbar != null)
            hotbar.ApplyConfig(config);

        if (hotbarUI != null)
            hotbarUI.ApplyConfig(config, equipment);
    }
}