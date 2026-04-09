using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int amount = 1;

    public ItemData ItemData
    {
        get => itemData;
        set => itemData = value;
    }

    public int Amount
    {
        get => amount;
        set => amount = value;
    }

    private void Start()
    {
        if (itemData == null)
            Debug.LogWarning("itemData is not assigned.", this);
    }
}