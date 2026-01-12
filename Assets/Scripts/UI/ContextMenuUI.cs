using UnityEngine;
using UnityEngine.UI;
using System;

public class ContextMenuUI : MonoBehaviour
{
    public Button useButton;
    public Button dropButton;
    public Button inspectButton;

    private int boundSlotIndex = -1;
    private Inventory boundInventory;

    void Awake()
    {
        // Button binding behavior (using lambda to delay binding slots)
        useButton.onClick.AddListener(() => { if (boundInventory != null) boundInventory.UseItem(boundSlotIndex); Hide(); });
        dropButton.onClick.AddListener(() => { if (boundInventory != null) boundInventory.DropItem(boundSlotIndex); Hide(); });
        inspectButton.onClick.AddListener(() => { if (boundInventory != null) boundInventory.InspectItem(boundSlotIndex); /* Keep open? close */ Hide(); });
    }

    public void ShowAt(Vector2 screenPos, Inventory inventory, int slotIndex)
    {
        boundInventory = inventory;
        boundSlotIndex = slotIndex;
        gameObject.SetActive(true);

        // Move to the mouse position
        var rt = (RectTransform)transform;
        rt.position = screenPos;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        boundInventory = null;
        boundSlotIndex = -1;
    }
}