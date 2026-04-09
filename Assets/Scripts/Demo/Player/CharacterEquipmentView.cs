using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipmentView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EquipmentVisualDatabase visualDatabase;
    [SerializeField] private Transform weaponSocket;

    private GameObject _currentWeapon;

    private void OnEnable()
    {
        InventoryEvents.OnEquipped += OnEquipped;
        InventoryEvents.OnUnequipped += OnUnequipped;
    }

    private void OnDisable()
    {
        InventoryEvents.OnEquipped -= OnEquipped;
        InventoryEvents.OnUnequipped -= OnUnequipped;
    }

    private void OnEquipped(EquipmentData item, List<StatModifier> _)
    {
        if (item == null || item.EquipSlot != EquipmentSlot.Weapon)
            return;

        var prefab = visualDatabase != null ? visualDatabase.GetPrefab(item.Id) : null;
        if (prefab == null)
            return;

        ClearWeapon();

        _currentWeapon = Instantiate(prefab, weaponSocket);
        _currentWeapon.transform.localPosition = Vector3.zero;
        _currentWeapon.transform.localRotation = Quaternion.identity;
    }

    private void OnUnequipped(EquipmentData item, List<StatModifier> _)
    {
        if (item == null || item.EquipSlot != EquipmentSlot.Weapon)
            return;

        ClearWeapon();
    }

    private void ClearWeapon()
    {
        if (_currentWeapon == null)
            return;

        Destroy(_currentWeapon);
        _currentWeapon = null;
    }
}