using UnityEngine;
using System.Collections.Generic;

public class InventoryUIController : MonoBehaviour
{
    public RectTransform panel;
    public CanvasGroup canvasGroup;
    public Inventory inventory;
    public InventorySlotUI[] slotsUI;

    bool isOpen;

    void Start()
    {
        for (int i = 0; i < slotsUI.Length; i++)
        {
            slotsUI[i].Setup(inventory, i);
        }
        
        canvasGroup.alpha = 0;
        panel.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        GameEvents.OnInventoryChanged += RefreshAll;
        GameEvents.OnEquipmentChanged += RefreshAll;
        RefreshAll();
    }

    void OnDisable()
    {
        GameEvents.OnInventoryChanged -= RefreshAll;
        GameEvents.OnEquipmentChanged -= RefreshAll;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool open = !isOpen;
            GameEvents.OnInventoryToggle?.Invoke(open);

            if (!open)
                GameEvents.OnInventoryClosed?.Invoke();

            if (isOpen) Close();
            else Open();
        }
    }

    void Open()
    {
        isOpen = true;
        panel.gameObject.SetActive(true);

        RefreshAll();

        LeanTween.moveX(panel, 0, 0.25f).setEaseOutCubic();
        LeanTween.alphaCanvas(canvasGroup, 1, 0.2f);
    }

    void Close()
    {
        isOpen = false;

        LeanTween.moveX(panel, 400, 0.25f).setEaseInCubic()
            .setOnComplete(() => panel.gameObject.SetActive(false));
        LeanTween.alphaCanvas(canvasGroup, 0, 0.2f);
    }

    void RefreshAll()
    {
        var visibleSet = new HashSet<int>(inventory.GetFilteredSlotIndices());

        for (int i = 0; i < slotsUI.Length; i++)
        {
            bool visible = visibleSet.Contains(i);
            slotsUI[i].SetVisible(visible);

            if (visible)
                slotsUI[i].Refresh();
        }
    }
}
