using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles item drops outside valid slots.
/// </summary>
public class GlobalDropArea : MonoBehaviour, IDropHandler
{
    [Header("Drag")]
    [SerializeField] private DraggableItemUI dragUI;

    public void OnDrop(PointerEventData eventData)
    {
        if (dragUI == null || dragUI.CurrentContext == null)
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
            if (ctx.Inventory.IsEquippedItemSourceSlot(ctx.InventorySlotIndex))
            {
                dragUI.EndDrag();
                return;
            }

            InventoryEvents.RemoveItemRequested?.Invoke(ctx.InventorySlotIndex, 1);
            InventoryEvents.ItemDropped?.Invoke(ctx.Item, 1);
        }

        dragUI.EndDrag();
    }
}
