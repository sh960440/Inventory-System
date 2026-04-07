using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A category button for the inventory filter that toggles the selected category and shows its active state.
/// </summary>
public class InventoryCategoryButton : MonoBehaviour
{
    [Header("Filter")]
    [SerializeField] private ItemCategory[] categories;

    [Header("UI")]
    [SerializeField] private Image highlightImage;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text buttonText;

    private Inventory _inventory;

    private void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(OnClick);
        InventoryEvents.InventoryChanged += RefreshState;
    }

    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClick);
        InventoryEvents.InventoryChanged -= RefreshState;
    }

    /// /// <summary>
    /// Initializes the category button with its label and the categories it represents.
    /// </summary>
    public void Initialize(Inventory inventory, ItemCategory[] categoryFilter, string label)
    {
        _inventory = inventory;
        categories = categoryFilter;
        if (buttonText != null)
            buttonText.text = label;
        RefreshState();
    }

    private void OnClick()
    {
        if (_inventory == null)
            return;
        _inventory.SetCategoryFilter(categories);
    }

    private void RefreshState()
    {
        if (highlightImage == null || _inventory == null)
            return;

        bool isActive =
            _inventory.CurrentCategories != null &&
            _inventory.CurrentCategories.Length == categories.Length &&
            _inventory.CurrentCategories.All(c => categories.Contains(c));

        highlightImage.gameObject.SetActive(isActive);
    }
}