using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryInputHandler : MonoBehaviour,
    InputSystem_Actions.IInventoryActions
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Hotbar hotbar;

    private InputSystem_Actions actions;

    void Awake()
    {
        actions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        actions.Enable();
        actions.Inventory.SetCallbacks(this);
        actions.Inventory.Enable();
    }

    void OnDisable()
    {
        actions.Inventory.SetCallbacks(null);
        actions.Disable();
    }

    private void UpdateActionMap()
    {
        if (inventory.IsOpen)
        {
            actions.Player.Disable();
            actions.UI.Enable();
        }
        else
        {
            actions.UI.Disable();
            actions.Player.Enable();
        }
    }

    // =========================
    // Inventory Actions
    // =========================

    public void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        inventory.SetOpen(!inventory.IsOpen);
        UpdateActionMap();
    }

    public void OnCloseInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        inventory.SetOpen(false);
        UpdateActionMap();
    }

    public void OnSplitStack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!inventory.IsOpen) return;

        int index = InventorySlotUI.CurrentHoveredIndex;
        if (index < 0) return;

        InventoryEvents.SplitStackRequested?.Invoke(index);
    }

    public void OnUseHotbar1(InputAction.CallbackContext context) => UseHotbar(0, context);
    public void OnUseHotbar2(InputAction.CallbackContext context) => UseHotbar(1, context);
    public void OnUseHotbar3(InputAction.CallbackContext context) => UseHotbar(2, context);
    public void OnUseHotbar4(InputAction.CallbackContext context) => UseHotbar(3, context);
    public void OnUseHotbar5(InputAction.CallbackContext context) => UseHotbar(4, context);
    public void OnUseHotbar6(InputAction.CallbackContext context) => UseHotbar(5, context);
    public void OnUseHotbar7(InputAction.CallbackContext context) => UseHotbar(6, context);
    public void OnUseHotbar8(InputAction.CallbackContext context) => UseHotbar(7, context);
    public void OnUseHotbar9(InputAction.CallbackContext context) => UseHotbar(8, context);

    // =========================
    // Core Logic
    // =========================

    private void UseHotbar(int index, InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!hotbar.ValidHotbarIndex(index))
            return;

        var slot = hotbar.GetInventorySlot(index);

        if (slot == null || slot.item == null)
        {
            hotbar.Clear(index);
            return;
        }

        InventoryEvents.HotbarUseRequested?.Invoke(slot);
    }
}