using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipmentView : MonoBehaviour
{
    [SerializeField] private EquipmentVisualDatabase visualDatabase;
    [SerializeField] private Transform weaponSocket;

    private GameObject currentWeapon;

    void OnEnable()
    {
        InventoryEvents.OnEquipped += OnEquipped;
        InventoryEvents.OnUnequipped += OnUnequipped;
    }

    void OnDisable()
    {
        InventoryEvents.OnEquipped -= OnEquipped;
        InventoryEvents.OnUnequipped -= OnUnequipped;
    }

    void OnEquipped(EquipmentData item, List<StatModifier> _)
    {
        if (item == null) return;
        if (item.equipSlot != EquipmentSlot.Weapon) return;

        var prefab = visualDatabase.GetPrefab(item.Id);
        if (prefab == null) return;

        ClearWeapon();

        currentWeapon = Instantiate(prefab, weaponSocket);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
    }

    void OnUnequipped(EquipmentData item, List<StatModifier> _)
    {
        if (item == null) return;
        if (item.equipSlot != EquipmentSlot.Weapon) return;

        ClearWeapon();
    }

    void ClearWeapon()
    {
        if (currentWeapon == null) return;

        Destroy(currentWeapon);
        currentWeapon = null;
    }
}