using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySortUI : MonoBehaviour
{
    public TMP_Dropdown sortTypeDropdown;
    public Button sortOrderButton;
    public Image sortOrderImage;

    public Sprite ascendingSprite;
    public Sprite descendingSprite;

    Inventory inventory;

    InventorySortType[] map =
    {
        InventorySortType.Name,
        InventorySortType.Rarity,
        InventorySortType.Category,
        InventorySortType.Count
    };

    void Awake()
    {
        inventory = FindFirstObjectByType<Inventory>();

        sortTypeDropdown.onValueChanged.AddListener(OnSortTypeChanged);
        sortOrderButton.onClick.AddListener(ToggleSortOrder);

        RefreshSortOrderIcon();
    }

    void OnSortTypeChanged(int index)
    {
        if (inventory == null) return;

        var selectedType = map[index];

        inventory.SetSort(selectedType, inventory.currentSortOrder);
        RefreshSortOrderIcon();
    }

    void ToggleSortOrder()
    {
        if (inventory == null) return;

        inventory.currentSortOrder =
            inventory.currentSortOrder == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending;

        inventory.SetSort(inventory.currentSortType, inventory.currentSortOrder);
        RefreshSortOrderIcon();
    }

    void RefreshSortOrderIcon()
    {
        if (sortOrderImage == null || inventory == null) return;

        sortOrderImage.sprite =
            inventory.currentSortOrder == SortOrder.Ascending
                ? ascendingSprite
                : descendingSprite;
    }
}
