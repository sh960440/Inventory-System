using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class InventoryCategoryButton : MonoBehaviour
{
    public ItemCategory[] categories;

    public Image highlightImage;
    public Button button;
    public TMP_Text buttonText;
    Inventory inventory;

    void Awake()
    {
        button.onClick.AddListener(OnClick);
        InventoryEvents.InventoryChanged += RefreshState;
    }

    void OnDestroy()
    {
        InventoryEvents.InventoryChanged -= RefreshState;
    }

    public void Initialize(Inventory inventory, ItemCategory[] categories, string label)
    {
        this.inventory = inventory;
        this.categories = categories;
        if (buttonText != null)
            buttonText.text = label;
        RefreshState();
    }

    void OnClick()
    {
        if (inventory == null) return;
        inventory.SetCategoryFilter(categories);
    }

    void RefreshState()
    {
        if (highlightImage == null || inventory == null) return;

        bool isActive =
            inventory.currentCategories != null &&
            inventory.currentCategories.Length == categories.Length &&
            inventory.currentCategories.All(c => categories.Contains(c));

        highlightImage.gameObject.SetActive(isActive);
    }
}