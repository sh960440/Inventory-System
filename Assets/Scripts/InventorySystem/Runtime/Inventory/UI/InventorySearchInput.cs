using UnityEngine;
using TMPro;

public class InventorySearchInput : MonoBehaviour
{
    public TMP_InputField input;
    public Inventory inventory;

    void Awake()
    {
        input.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(string text)
    {
        if (inventory == null) return;
        inventory.SetSearchKeyword(text);
    }
}