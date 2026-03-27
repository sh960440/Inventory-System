using System.Collections.Generic;

/// Narrow surface for inventory UI (list/query/filter + swap/stack from drag-drop).
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