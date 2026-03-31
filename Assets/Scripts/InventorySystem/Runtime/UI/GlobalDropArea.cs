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

        // Hotbar: clear slot binding only
        if (eventData.pointerDrag.TryGetComponent<HotbarSlotUI>(out var hotbarSlot))
        {
            hotbarSlot.ClearSelf();
        }
        // Inventory: remove from slot + drop item
        else if (ctx.Inventory != null && ctx.InventorySlotIndex >= 0)
        {
            InventoryEvents.RemoveItemRequested?.Invoke(ctx.InventorySlotIndex, 1);
            InventoryEvents.ItemDropped?.Invoke(ctx.Item, 1);
        }

        dragUI.EndDrag();
    }
}
