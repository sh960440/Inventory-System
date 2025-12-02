using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    private ItemPickup currentPickup;
    private Inventory playerInventory;

    private void Start()
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
        Debug.Log("Press E to pick " + other.gameObject.name);
        var pickup = other.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            currentPickup = pickup;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Get closer");
        if (other.GetComponent<ItemPickup>() == currentPickup)
        {
            currentPickup = null;
        }    
    }

    private void PickupItem()
    {
        Debug.Log("Checking...");
        if (playerInventory == null)
            Debug.Log("playerInventory == null");
        if (currentPickup == null)
            Debug.Log("currentPickup == null");

        if (playerInventory == null || currentPickup == null) return;

        Debug.Log("Picking...");
        playerInventory.AddItem(currentPickup.itemData, currentPickup.amount);

        Destroy(currentPickup.gameObject);
        currentPickup = null;
    }
}
