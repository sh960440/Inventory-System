using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InventoryCategoryButton : MonoBehaviour
{
    public ItemCategory[] categories;

    public Image highlight;
    Inventory inventory;
    Button button;

    void Awake()
    {
        inventory = FindFirstObjectByType<Inventory>();
        button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);
        InventoryEvents.InventoryChanged += RefreshState;
        RefreshState();
    }

    void OnDestroy()
    {
        InventoryEvents.InventoryChanged -= RefreshState;
    }

    void OnClick()
    {
        if (inventory == null) return;
        inventory.SetCategoryFilter(categories);
    }

    void RefreshState()
    {
        if (highlight == null || inventory == null) return;

        bool isActive =
            inventory.currentCategories != null &&
            inventory.currentCategories.Length == categories.Length &&
            inventory.currentCategories.All(c => categories.Contains(c));

        highlight.gameObject.SetActive(isActive);
    }
}