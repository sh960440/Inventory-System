using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalDropArea : MonoBehaviour, IDropHandler
{
    [SerializeField] DraggableItemUI dragUI;

    public void OnDrop(PointerEventData eventData)
    {
        if (dragUI.CurrentContext == null)
            return;

        var ctx = dragUI.CurrentContext.Value;

        // Hotbar
        if (eventData.pointerDrag.TryGetComponent<HotbarSlotUI>(out var hotbarSlot))
        {
            hotbarSlot.ClearSelf();
        }
        // Inventory
        else
        {
            //ctx.inventory.DropItem(ctx.inventorySlotIndex, 1);
            InventoryEvents.ItemRemoved?.Invoke(ctx.inventorySlotIndex, 1);
            InventoryEvents.ItemDropped?.Invoke(ctx.item, 1);
        }

        dragUI.EndDrag();
    }
}
