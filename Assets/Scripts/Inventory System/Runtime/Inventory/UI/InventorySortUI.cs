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

    public Inventory inventory;

    InventorySortType[] map =
    {
        InventorySortType.Name,
        InventorySortType.Rarity,
        InventorySortType.Category,
        InventorySortType.Count
    };

    void Awake()
    {
        sortTypeDropdown.onValueChanged.AddListener(OnSortTypeChanged);
        sortOrderButton.onClick.AddListener(ToggleSortOrder);

        Initialize();
    }

    void Initialize()
    {
        if (inventory == null) return;
        var currentType = map[sortTypeDropdown.value];
        inventory.SetSort(currentType, inventory.CurrentSortOrder);
        RefreshSortOrderIcon();
    }

    void OnSortTypeChanged(int index)
    {
        if (inventory == null) return;

        var selectedType = map[index];

        inventory.SetSort(selectedType, inventory.CurrentSortOrder);
        RefreshSortOrderIcon();
    }

    void ToggleSortOrder()
    {
        if (inventory == null) return;

        inventory.ToggleSortOrder();
        RefreshSortOrderIcon();
    }

    void RefreshSortOrderIcon()
    {
        if (sortOrderImage == null || inventory == null) return;

        sortOrderImage.sprite =
            inventory.CurrentSortOrder == SortOrder.Ascending
                ? ascendingSprite
                : descendingSprite;
    }
}
