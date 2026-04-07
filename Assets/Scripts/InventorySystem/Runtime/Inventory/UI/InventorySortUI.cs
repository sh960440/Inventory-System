using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the inventory sorting UI, including the sort dropdown and order toggle.
/// </summary>
public class InventorySortUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown sortTypeDropdown;
    [SerializeField] private Button sortOrderButton;
    [SerializeField] private Image sortOrderImage;

    [Header("Icons")]
    [SerializeField] private Sprite ascendingSprite;
    [SerializeField] private Sprite descendingSprite;

    [Header("References")]
    [SerializeField] private Inventory inventory;

    private static readonly InventorySortType[] SortTypeMap =
    {
        InventorySortType.Name,
        InventorySortType.Rarity,
        InventorySortType.Category,
        InventorySortType.Count
    };

    private void OnEnable()
    {
        if (sortTypeDropdown != null)
            sortTypeDropdown.onValueChanged.AddListener(OnSortTypeChanged);
        if (sortOrderButton != null)
            sortOrderButton.onClick.AddListener(ToggleSortOrder);

        SyncFromInventory();
    }

    private void OnDisable()
    {
        if (sortTypeDropdown != null)
            sortTypeDropdown.onValueChanged.RemoveListener(OnSortTypeChanged);
        if (sortOrderButton != null)
            sortOrderButton.onClick.RemoveListener(ToggleSortOrder);
    }

    private void SyncFromInventory()
    {
        if (inventory == null || sortTypeDropdown == null)
            return;

        var currentType = SortTypeMap[sortTypeDropdown.value];
        inventory.SetSort(currentType, inventory.CurrentSortOrder);
        RefreshSortOrderIcon();
    }

    private void OnSortTypeChanged(int index)
    {
        if (inventory == null)
            return;

        var selectedType = SortTypeMap[index];
        inventory.SetSort(selectedType, inventory.CurrentSortOrder);
        RefreshSortOrderIcon();
    }

    private void ToggleSortOrder()
    {
        if (inventory == null)
            return;

        inventory.ToggleSortOrder();
        RefreshSortOrderIcon();
    }

    private void RefreshSortOrderIcon()
    {
        if (sortOrderImage == null || inventory == null)
            return;

        sortOrderImage.sprite =
            inventory.CurrentSortOrder == SortOrder.Ascending
                ? ascendingSprite
                : descendingSprite;
    }
}