using UnityEngine;
using System;

public class EquipmentUIController : MonoBehaviour
{
    public Equipment equipmentManager;
    public EquipmentSlotUI slotPrefab;
    public Transform container;
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

    void Start()
    {
        BuildUI();
    }

    void BuildUI()
    {
        int equipmentCount = Enum.GetValues(typeof(EquipmentSlot)).Length;
        slots = new EquipmentSlotUI[equipmentCount];

        for (int i = 0; i < equipmentCount; i++)
        {
            var ui = Instantiate(slotPrefab, container);
            ui.slotType = (EquipmentSlot)i;
            ui.equipmentManager = equipmentManager;
            slots[i] = ui;
        }
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