using UnityEngine;

/// <summary>
/// Builds hotbar slot UIs and refreshes them when bindings change.
/// </summary>
public class HotbarUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Hotbar hotbar;
    [SerializeField] private HotbarSlotUI slotPrefab;
    [SerializeField] private DraggableItemUI dragUI;
    [SerializeField] private Transform container;

    private HotbarSlotUI[] _slotUIs;

    private void OnEnable()
    {
        InventoryEvents.HotbarChanged += Refresh;
        InventoryEvents.InventoryChanged += Refresh;
    }

    private void OnDisable()
    {
        InventoryEvents.HotbarChanged -= Refresh;
        InventoryEvents.InventoryChanged -= Refresh;
    }

    /// <summary>
    /// Creates the hotbar slot UIs and connects them to the underlying data.
    /// </summary>
    public void ApplyConfig(ItemSystemConfiguration config, Equipment equipmentManager)
    {
        if (dragUI != null)
            dragUI.SetPointerOffset(config.DragIconOffset);

        ClearSlotInstances();

        _slotUIs = new HotbarSlotUI[config.HotkeyCount];

        for (int i = 0; i < config.HotkeyCount; i++)
        {
            var ui = Instantiate(slotPrefab, container);
            ui.Setup(hotbar, equipmentManager, i, dragUI);
            _slotUIs[i] = ui;
        }
    }

    private void ClearSlotInstances()
    {
        if (container == null)
            return;

        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);

        _slotUIs = null;
    }

    private void Refresh()
    {
        if (_slotUIs == null)
            return;

        foreach (var ui in _slotUIs)
            ui?.Refresh();
    }
}