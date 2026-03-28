public sealed class EquipmentUseHandler : IItemUseHandler
{
    public bool CanUse(ItemData item) => item is EquipmentData;

    public void Use(ItemUseContext context, InventorySlot slot)
    {
        if (slot?.item is not EquipmentData eq)
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