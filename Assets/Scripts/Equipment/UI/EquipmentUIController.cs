using UnityEngine;

public class EquipmentUIController : MonoBehaviour
{
    public EquipmentManager equipmentManager;
    public EquipmentSlotUI[] slots;

    void Awake()
    {
        GameEvents.OnEquipped += OnEquipmentChanged;
        GameEvents.OnUnequipped += OnEquipmentChanged;
        GameEvents.OnEquipmentChanged += OnEquipmentChanged;
    }

    void OnDestroy()
    {
        GameEvents.OnEquipped -= OnEquipmentChanged;
        GameEvents.OnUnequipped -= OnEquipmentChanged;
        GameEvents.OnEquipmentChanged -= OnEquipmentChanged;
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