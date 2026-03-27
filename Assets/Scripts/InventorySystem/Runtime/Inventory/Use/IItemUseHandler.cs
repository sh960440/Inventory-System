public interface IItemUseHandler
{
    bool CanUse(ItemData item);
    void Use(ItemUseContext context, InventorySlot slot);
}