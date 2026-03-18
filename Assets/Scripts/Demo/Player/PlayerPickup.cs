using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    private ItemPickup currentPickup;
    public Inventory playerInventory;
    public ObjectPool pool;

    public void TryPickup()
    {
        if (currentPickup != null)
        {
            PickupItem();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var ip = other.GetComponent<ItemPickup>();
        if (ip != null)
        {
            currentPickup = ip;
            Debug.Log("Enter pickup range: " + ip.itemData.itemName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var ip = other.GetComponent<ItemPickup>();
        if (ip != null && ip == currentPickup)
        {
            currentPickup = null;
            Debug.Log("Exit pickup range");
        }
    }

    private void PickupItem()
    {
        if (playerInventory == null || currentPickup == null) return;

        //playerInventory.AddItem(currentPickup.itemData, currentPickup.amount);
        InventoryEvents.ItemAdded?.Invoke(currentPickup.itemData, currentPickup.amount);

        var prefab = currentPickup.itemData.worldPrefab;
        pool.Return(prefab, currentPickup.gameObject);

        currentPickup = null;
    }
}
