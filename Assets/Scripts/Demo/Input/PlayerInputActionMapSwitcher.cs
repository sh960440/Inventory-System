using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Switches PlayerInput's action map when inventory opens/closes.
/// Keeps movement disabled while inventory UI is open.
/// </summary>
public class PlayerInputActionMapSwitcher : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Inventory inventory;

    void OnEnable()
    {
        InventoryEvents.InventoryToggleRequested += OnInventoryToggled;
        InventoryEvents.InventoryCloseRequested += OnInventoryClosed;
        UpdateActionMap();
    }

    void OnDisable()
    {
        InventoryEvents.InventoryToggleRequested -= OnInventoryToggled;
        InventoryEvents.InventoryCloseRequested -= OnInventoryClosed;
    }

    void OnInventoryToggled(bool open) => UpdateActionMap();
    void OnInventoryClosed() => UpdateActionMap();

    void UpdateActionMap()
    {
        if (playerInput == null || inventory == null) return;
        playerInput.SwitchCurrentActionMap(inventory.IsOpen ? "UI" : "Player");
    }
}