/// <summary>
/// Handles using consumable items by applying their effects and reducing the stack count.
/// </summary>
public sealed class ConsumableUseHandler : IItemUseHandler
{
    /// <inheritdoc />
    public bool CanUse(ItemData item) => item is ConsumableData;

    /// <inheritdoc />
    public void Use(ItemUseContext context, InventorySlot slot)
    {
        if (slot?.Item is not ConsumableData consumable)
            return;

        InventoryEvents.ItemConsumed?.Invoke(consumable);
        context.Inventory.RemoveItem(slot);
    }
}