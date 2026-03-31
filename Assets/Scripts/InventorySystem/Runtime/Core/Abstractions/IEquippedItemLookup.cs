/// <summary>
/// Determines whether an inventory slot is the source cell for an equipped item.
/// </summary>
public interface IEquippedItemLookup
{
    /// <summary>Returns true when the slot holds the equipped instance reference for this equipment entry.</summary>
    bool IsInventorySlotSourceOfEquippedItem(int inventorySlotIndex, EquipmentData item);
}