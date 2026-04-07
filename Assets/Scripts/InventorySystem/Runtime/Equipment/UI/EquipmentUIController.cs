using System;
using UnityEngine;

/// <summary>
/// Instantiates one EquipmentSlotUI per EquipmentSlot and binds them to Equipment.
/// </summary>
public class EquipmentUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Equipment equipmentManager;
    [SerializeField] private EquipmentSlotUI slotPrefab;
    [SerializeField] private Transform container;

    private EquipmentSlotUI[] _slots;

    private void Start()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        int count = Enum.GetValues(typeof(EquipmentSlot)).Length;
        _slots = new EquipmentSlotUI[count];

        for (int i = 0; i < count; i++)
        {
            var ui = Instantiate(slotPrefab, container);
            ui.Configure((EquipmentSlot)i, equipmentManager);
            _slots[i] = ui;
        }
    }
}