using UnityEngine;

/// <summary>
/// A inventory grid cell that stores an item reference and its quantity.
/// </summary>
[System.Serializable]
public class InventorySlot
{
    [SerializeField] private ItemData item;
    [SerializeField] private int count;

    public ItemData Item
    {
        get => item;
        set => item = value;
    }

    public int Count
    {
        get => count;
        set => count = value;
    }

    public InventorySlot(ItemData item = null, int count = 0)
    {
        Item = item;
        Count = count;
    }
}