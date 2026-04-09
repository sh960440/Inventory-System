using UnityEngine;

public class CharacterEquipmentVisual : MonoBehaviour
{
    [Header("Socket")]
    [SerializeField] private Transform rightHand;

    private GameObject _currentWeapon;

    public void EquipWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null || rightHand == null)
            return;

        if (_currentWeapon != null)
            Destroy(_currentWeapon);

        _currentWeapon = Instantiate(weaponPrefab, rightHand);
        _currentWeapon.transform.localPosition = Vector3.zero;
        _currentWeapon.transform.localRotation = Quaternion.identity;
    }
}