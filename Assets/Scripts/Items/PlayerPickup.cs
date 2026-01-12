using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    private ItemPickup currentPickup;
    private Inventory playerInventory;

    void Start()
    {
        playerInventory = transform.parent.GetComponentInChildren<Inventory>();
    }

    void Update()
    {
        if (currentPickup != null && Input.GetKeyDown(KeyCode.E))
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
        GameEvents.OnItemPicked?.Invoke(currentPickup.itemData, currentPickup.amount);
        Debug.Log($"Picked up {currentPickup.itemData.itemName} x{currentPickup.amount}");
        Destroy(currentPickup.gameObject);
        currentPickup = null;
    }
}
