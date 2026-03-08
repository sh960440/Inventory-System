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

    public void ApplyConfig(ItemSystemConfiguration config)
    {
        slotUIs = new HotbarSlotUI[config.hotkeyCount];

        for (int i = 0; i < config.hotkeyCount; i++)
        {
            var ui = Instantiate(slotPrefab, container);
            ui.Setup(hotbar, i);
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