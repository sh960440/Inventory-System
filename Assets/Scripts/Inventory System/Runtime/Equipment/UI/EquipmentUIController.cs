using UnityEngine;

public class EquipmentUIController : MonoBehaviour
{
    public Equipment equipmentManager;
    public EquipmentSlotUI[] slots;

    void Awake()
    {
        InventoryEvents.OnEquipped += OnEquipmentChanged;
        InventoryEvents.OnUnequipped += OnEquipmentChanged;
        InventoryEvents.EquipmentChanged += OnEquipmentChanged;
    }

    void OnDestroy()
    {
        InventoryEvents.OnEquipped -= OnEquipmentChanged;
        InventoryEvents.OnUnequipped -= OnEquipmentChanged;
        InventoryEvents.EquipmentChanged -= OnEquipmentChanged;
    }

    void OnEquipmentChanged(EquipmentData _, System.Collections.Generic.List<StatModifier> __)
    {
        Refresh();
    }

    void OnEquipmentChanged()
    {
        Refresh();
    }

    void Refresh()
    {
        //if (equipmentManager == null) return;

        //foreach (var slotUI in slots)
        //{
        //    var item = equipmentManager.GetEquipped(slotUI.slotType);
        //    slotUI.SetItem(item);
        //}
    }
}