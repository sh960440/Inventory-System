using TMPro;
using UnityEngine;

/// <summary>
/// Sends the search text to the inventory so it can update its filter.
/// </summary>
public class InventorySearchInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField input;
    [SerializeField] private Inventory inventory;

    private void OnEnable()
    {
        if (input != null)
            input.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDisable()
    {
        if (input != null)
            input.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(string text)
    {
        if (inventory == null)
            return;
        inventory.SetSearchKeyword(text);
    }
}