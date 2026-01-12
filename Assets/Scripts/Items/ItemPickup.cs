using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1;

    void Start()
    {
        if (itemData == null)
            Debug.LogWarning("itemData doesn't exist");
    }
}
