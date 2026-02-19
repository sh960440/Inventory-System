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
        BuildUI();
    }

    void OnEnable()
    {
        InventoryEvents.OnHotbarChanged += Refresh;
        InventoryEvents.OnInventoryChanged += Refresh;
    }

    void OnDisable()
    {
        InventoryEvents.OnHotbarChanged -= Refresh;
        InventoryEvents.OnInventoryChanged -= Refresh;
    }

    void BuildUI()
    {
        slotUIs = new HotbarSlotUI[hotbar.slots.Count];

        for (int i = 0; i < hotbar.slots.Count; i++)
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