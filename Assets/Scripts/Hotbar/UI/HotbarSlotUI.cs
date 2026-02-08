using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HotbarSlotUI : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public Image icon;
    public TMP_Text keyText;

    Hotbar hotbar;
    int index;

    DragItemContext? currentDrag;

    void OnEnable()
    {
        GameEvents.OnItemDragBegin += OnDragBegin;
        GameEvents.OnItemDragEnd += OnDragEnd;
    }

    void OnDisable()
    {
        GameEvents.OnItemDragBegin -= OnDragBegin;
        GameEvents.OnItemDragEnd -= OnDragEnd;
    }

    void OnDragBegin(DragItemContext ctx)
    {
        currentDrag = ctx;
    }

    void OnDragEnd()
    {
        currentDrag = null;
    }

    public void Setup(Hotbar hb, int slotIndex)
    {
        hotbar = hb;
        index = slotIndex;
        keyText.text = (slotIndex + 1).ToString(); // Display key number
    }

    public void Refresh()
    {
        var invSlot = hotbar.GetInventorySlot(index);

        if (invSlot == null || invSlot.item == null)
        {
            icon.enabled = false;
            return;
        }

        icon.sprite = invSlot.item.icon;
        icon.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;

        Debug.Log($"Hotbar slot {index} clicked");
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (currentDrag == null) return;

        var ctx = currentDrag.Value;

        if (ctx.item == null) return;

        hotbar.Assign(
            index,
            ctx.inventory,
            ctx.inventorySlotIndex
        );
    }
}
