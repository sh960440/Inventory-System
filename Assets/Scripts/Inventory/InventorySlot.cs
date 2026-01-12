using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int count;

    public InventorySlot(ItemData item = null, int count = 0)
    {
        this.item = item;
        this.count = count;
    }
}
