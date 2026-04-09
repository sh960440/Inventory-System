using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputActionMapSwitcher : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Inventory inventory;

    private void OnEnable()
    {
        InventoryEvents.InventoryToggleRequested += OnInventoryToggled;
        InventoryEvents.InventoryCloseRequested += OnInventoryClosed;
        UpdateActionMap();
    }

    private void OnDisable()
    {
        InventoryEvents.InventoryToggleRequested -= OnInventoryToggled;
        InventoryEvents.InventoryCloseRequested -= OnInventoryClosed;
    }

    private void OnInventoryToggled(bool _) => UpdateActionMap();
    private void OnInventoryClosed() => UpdateActionMap();

    private void UpdateActionMap()
    {
        if (playerInput == null || inventory == null) 
            return;
        playerInput.SwitchCurrentActionMap(inventory.IsOpen ? "UI" : "Player");
    }
}