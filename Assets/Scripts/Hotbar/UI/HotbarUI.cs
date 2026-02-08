using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    public Hotbar hotbar;
    public HotbarSlotUI slotPrefab;
    public Transform container;

    HotbarSlotUI[] slotUIs;

    void Start()
    {
        BuildUI();
    }

    void OnEnable()
    {
        GameEvents.OnHotbarChanged += Refresh;
        GameEvents.OnInventoryChanged += Refresh;
    }

    void OnDisable()
    {
        GameEvents.OnHotbarChanged -= Refresh;
        GameEvents.OnInventoryChanged -= Refresh;
    }

    void BuildUI()
    {
        slotUIs = new HotbarSlotUI[hotbar.slots.Count];

        for (int i = 0; i < hotbar.slots.Count; i++)
        {
            var ui = Instantiate(slotPrefab, container);
            ui.Setup(hotbar, i);
            slotUIs[i] = ui;
        }
    }

    void Refresh()
    {
        foreach (var ui in slotUIs)
            ui.Refresh();
    }
}