using System;
using UnityEngine;

/// <summary>
/// A hotbar slot that remembers which item instance it is linked to.
/// </summary>
[System.Serializable]
public class HotbarSlot
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemData item;
    [NonSerialized] private InventorySlot boundInventoryCell;

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

    public bool IsEmpty => item == null;

    public InventorySlot BoundInventoryCell
    {
        get => boundInventoryCell;
        set => boundInventoryCell = value;
    }

    public void Clear()
    {
        item = null;
        boundInventoryCell = null;
        inventory = null;
    }
}