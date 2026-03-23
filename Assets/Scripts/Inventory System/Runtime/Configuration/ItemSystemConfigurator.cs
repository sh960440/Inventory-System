using System.Collections.Generic;
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
    [SerializeField] private SlotHoverService slotHoverService;

    [Header("Extension Points")]
    [Tooltip("Optional item use handlers. Any component here that implements IItemUseHandler will be registered in order.")]
    [SerializeField] private List<MonoBehaviour> itemUseHandlers = new List<MonoBehaviour>();

    private void Awake()
    {
        ApplyConfig();
    }

    private void ApplyConfig()
    {
        if (slotHoverService == null)
            slotHoverService = GetComponent<SlotHoverService>() ?? gameObject.AddComponent<SlotHoverService>();

        if (inventory != null)
            inventory.ApplyConfig(config, equipment);

        if (inventory != null)
        {
            inventory.ClearUseHandlers();

            for (int i = 0; i < itemUseHandlers.Count; i++)
            {
                if (itemUseHandlers[i] is IItemUseHandler h)
                    inventory.RegisterUseHandler(h);
            }

            inventory.EnsureUseHandlers();
        }

        if (inventoryUI != null)
            inventoryUI.ApplyConfig(config, equipment, slotHoverService);

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