/// <summary>
/// Defines logic for determining whether an item can be used and performing its use action.
/// </summary>
public interface IItemUseHandler
{
    /// <summary>
    /// Returns whether this handler should run for the given item definition.
    /// </summary>
    bool CanUse(ItemData item);

    /// <summary>
    /// Runs the use action for the slot.
    /// </summary>
    void Use(ItemUseContext context, InventorySlot slot);
}