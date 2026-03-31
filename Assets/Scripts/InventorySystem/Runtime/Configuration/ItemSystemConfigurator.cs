using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Applies ItemSystemConfiguration to inventory, equipment, hotbar, and UI controllers at startup.
/// </summary>
public class ItemSystemConfigurator : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ItemSystemConfiguration config;

    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Equipment equipment;
    [SerializeField] private Hotbar hotbar;
    [SerializeField] private InventoryUIController inventoryUI;
    [SerializeField] private EquipmentUIController equipmentUI;
    [SerializeField] private HotbarUIController hotbarUI;
    [SerializeField] private SlotHoverService slotHoverService;

    [Header("Inventory UI Wiring")]
    [SerializeField] private RectTransform inventoryPanel;
    [SerializeField] private CanvasGroup inventoryCanvasGroup;
    [SerializeField] private InventorySlotUI inventorySlotPrefab;
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private GridLayoutGroup inventoryGridLayout;
    [SerializeField] private Transform inventoryCategoryButtonContainer;
    [SerializeField] private InventoryCategoryButton inventoryCategoryButtonPrefab;
    [SerializeField] private ContextMenuUI inventoryContextMenuUI;
    [SerializeField] private DraggableItemUI inventoryDragUI;
    [SerializeField] private TooltipUI inventoryTooltipUI;

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

        if (equipment != null && inventory != null)
            equipment.BindInventory(inventory);

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
        {
            inventoryUI.ApplyWiring(new InventoryUIController.InventoryUIWiring
            {
                panel = inventoryPanel,
                canvasGroup = inventoryCanvasGroup,
                slotPrefab = inventorySlotPrefab,
                container = inventoryContainer,
                gridLayout = inventoryGridLayout,

                categoryButtonContainer = inventoryCategoryButtonContainer,
                categoryButtonPrefab = inventoryCategoryButtonPrefab,

                contextMenuUI = inventoryContextMenuUI,
                dragUI = inventoryDragUI,
                tooltipUI = inventoryTooltipUI
            });

            inventoryUI.ApplyConfig(config, equipment, slotHoverService);
        }

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