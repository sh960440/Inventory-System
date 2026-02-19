using UnityEngine;
using TMPro;

public class InventorySearchInput : MonoBehaviour
{
    public TMP_InputField input;
    Inventory inventory;

    void Awake()
    {
        inventory = FindFirstObjectByType<Inventory>();
        input.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(string text)
    {
        if (inventory == null) return;
        inventory.SetSearchKeyword(text);
    }
}