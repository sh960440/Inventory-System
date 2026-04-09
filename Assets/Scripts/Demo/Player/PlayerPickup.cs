using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ObjectPool pool;

    private ItemPickup _currentPickup;

    public void TryPickup()
    {
        if (_currentPickup != null)
            PickupItem();
    }

    private void OnTriggerEnter(Collider other)
    {
        var ip = other.GetComponent<ItemPickup>();
        if (ip != null)
            _currentPickup = ip;
    }

    private void OnTriggerExit(Collider other)
    {
        var ip = other.GetComponent<ItemPickup>();
        if (ip != null && ip == _currentPickup)
            _currentPickup = null;
    }

    private void PickupItem()
    {
        if (playerInventory == null || _currentPickup == null || pool == null)
            return;

        var data = _currentPickup.ItemData;
        if (data == null)
            return;

        InventoryEvents.AddItemRequested?.Invoke(data, _currentPickup.Amount);

        pool.Return(data.WorldPrefab, _currentPickup.gameObject);

        _currentPickup = null;
    }
}