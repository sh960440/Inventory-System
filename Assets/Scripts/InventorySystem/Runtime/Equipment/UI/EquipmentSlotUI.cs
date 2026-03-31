using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotUI : UISlotBase, IPointerEnterHandler, IPointerExitHandler
{
    public EquipmentSlot slotType;   // Head / Weapon / etc.
    public Equipment equipmentManager;
    public Image iconImage;
    EquipmentData currentItem;

    void OnEnable()
    {
        InventoryEvents.EquipmentChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        InventoryEvents.EquipmentChanged -= Refresh;
    }

    void Refresh()
    {
        if (equipmentManager == null) return;   

        var item = equipmentManager.GetEquipped(slotType);
        currentItem = item;

        if (item == null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            return;
        }

        iconImage.sprite = item.icon;
        iconImage.enabled = true;
        iconImage.preserveAspect = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem == null) return;
        InventoryEvents.TooltipRequested?.Invoke(new ItemUIContext(
            currentItem,
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
        if (currentItem == null) return;
        InventoryEvents.UnequipRequested?.Invoke(currentItem.equipSlot);
    }
}