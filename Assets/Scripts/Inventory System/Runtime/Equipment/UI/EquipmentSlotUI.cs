using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public EquipmentSlot slotType;   // Head / Weapon / etc.
    public Equipment equipmentManager;
    public Image iconImage;
    EquipmentData currentItem;

    void OnEnable()
    {
        InventoryEvents.OnEquipmentChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        InventoryEvents.OnEquipmentChanged -= Refresh;
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
        InventoryEvents.OnTooltipRequest?.Invoke(new ItemUIContext
        {
            item = currentItem,
            slotIndex = -1,
            isFromInventory = false,
            isEquipped = true,
            count = -1
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryEvents.OnTooltipHide?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (currentItem == null) return;

        InventoryEvents.OnContextMenuRequest?.Invoke(new ItemUIContext
        {
            item = currentItem,
            isFromInventory = false,
            isEquipped = true,
            slotIndex = -1,
            count = -1
        });
    }
}