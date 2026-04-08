using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// /// <summary>
/// An equipment slot UI that displays the equipped item and supports basic interactions.
/// </summary>
public class EquipmentSlotUI : UISlotBase, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Equipment")]
    [SerializeField] private EquipmentSlot slotType;
    [SerializeField] private Equipment equipmentManager;

    [Header("UI")]
    [SerializeField] private Image iconImage;

    private EquipmentData _currentItem;

    /// <summary>
    /// Wired by EquipmentUIController when _slots are built.
    /// </summary>
    public void Configure(EquipmentSlot slot, Equipment equipment)
    {
        slotType = slot;
        equipmentManager = equipment;
    }

    private void OnEnable()
    {
        InventoryEvents.EquipmentChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        InventoryEvents.EquipmentChanged -= Refresh;
    }

    private void Refresh()
    {
        if (equipmentManager == null)
            return;   

        var item = equipmentManager.GetEquipped(slotType);
        _currentItem = item;

        if (item == null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            return;
        }

        iconImage.sprite = item.Icon;
        iconImage.enabled = true;
        iconImage.preserveAspect = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentItem == null) 
            return;

        InventoryEvents.TooltipRequested?.Invoke(new ItemUIContext(
            _currentItem,
            isFromInventory: false,
            isEquipped: true,
            slotIndex: -1,
            stackCount: -1));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryEvents.TooltipHidden?.Invoke();
    }

    protected override void OnDoubleClick()
    {
        if (_currentItem == null)
            return;

        InventoryEvents.UnequipRequested?.Invoke(_currentItem.EquipSlot);
    }
}