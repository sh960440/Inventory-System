/// <summary>
/// Handles using equipment items by equipping them or requesting an unequip action.
/// </summary>
public sealed class EquipmentUseHandler : IItemUseHandler
{
    /// <inheritdoc />
    public bool CanUse(ItemData item) => item is EquipmentData;

    /// <inheritdoc />
    public void Use(ItemUseContext context, InventorySlot slot)
    {
        if (slot?.Item is not EquipmentData eq)
            return;

        var equipment = context.Equipment;
        if (equipment == null)
            return;

        if (equipment.IsInventorySlotSourceOfEquippedItem(context.InventorySlotIndex, eq))
            InventoryEvents.UnequipRequested?.Invoke(eq.equipSlot);
        else
            InventoryEvents.EquipRequested?.Invoke(eq, context.InventorySlotIndex);
    }
}