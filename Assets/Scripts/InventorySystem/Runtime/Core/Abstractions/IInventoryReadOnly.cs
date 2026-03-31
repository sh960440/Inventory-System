using System.Collections.Generic;

/// <summary>
/// Read-only inventory surface for UI.
/// </summary>
public interface IInventoryReadOnly
{
    IReadOnlyList<InventorySlot> Slots { get; }

    int SlotCount { get; }

    bool AllowDoubleClickUse { get; }

    bool Valid(int index);

    void GetFilteredSlotIndices(List<int> result);

    bool PassFilter(InventorySlot slot);

    bool TrySwapOrStack(int fromIndex, int toIndex);
}