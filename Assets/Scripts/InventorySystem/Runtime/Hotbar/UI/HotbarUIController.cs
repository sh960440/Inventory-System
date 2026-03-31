using UnityEngine;

public class HotbarUIController : MonoBehaviour
{
    public Hotbar hotbar;
    public HotbarSlotUI slotPrefab;
    public DraggableItemUI dragUI;
    public Transform container;

    HotbarSlotUI[] slotUIs;

    void Start()
    {
        
    }

    void OnEnable()
    {
        InventoryEvents.HotbarChanged += Refresh;
        InventoryEvents.InventoryChanged += Refresh;
    }

    void OnDisable()
    {
        InventoryEvents.HotbarChanged -= Refresh;
        InventoryEvents.InventoryChanged -= Refresh;
    }

    public void ApplyConfig(ItemSystemConfiguration config, Equipment equipmentManager)
    {
        slotUIs = new HotbarSlotUI[config.HotkeyCount];

        for (int i = 0; i < config.HotkeyCount; i++)
        {
            var ui = Instantiate(slotPrefab, container);
            ui.Setup(hotbar, equipmentManager, i);
            ui.SetDragUI(dragUI);
            slotUIs[i] = ui;
        }
    }

    void Refresh()
    {
        foreach (var ui in slotUIs)
            ui.Refresh();
    }
}