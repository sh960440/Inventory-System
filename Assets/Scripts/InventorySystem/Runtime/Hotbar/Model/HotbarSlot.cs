using UnityEngine;

/// <summary>
/// A hotbar slot that remembers which item instance it is linked to.
/// </summary>
[System.Serializable]
public class HotbarSlot
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemData item;
    [SerializeField] private int boundInventorySlotIndex = -1;

    public Inventory Inventory
    {
        get => inventory;
        set => inventory = value;
    }

    public ItemData Item
    {
        get => item;
        set => item = value;
    }

    public int BoundInventorySlotIndex
    {
        get => boundInventorySlotIndex;
        set => boundInventorySlotIndex = value;
    }

    public bool IsEmpty => item == null;

    public void Clear()
    {
        item = null;
        boundInventorySlotIndex = -1;
        inventory = null;
    }
}