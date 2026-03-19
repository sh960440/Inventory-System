public sealed class ConsumableUseHandler : IItemUseHandler
{
    public bool CanUse(ItemData item) => item is ConsumableData;

    public void Use(ItemUseContext context, InventorySlot slot)
    {
        if (slot?.item is not ConsumableData consumable)
            return;

        InventoryEvents.ItemConsumed?.Invoke(consumable);
        context.Inventory.RemoveItem(slot);
        InventoryEvents.InventoryChanged?.Invoke();
    }
}