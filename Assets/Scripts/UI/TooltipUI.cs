using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text countText;
    public TMP_Text descriptionText;
    public Image background;
    public CanvasGroup canvasGroup;
    bool isFollowingMouse = false;

    void Awake()
    {
        GameEvents.OnSlotHovered += ShowForSlot;
        GameEvents.OnSlotHoverExit += Hide;
        GameEvents.OnInventoryClosed += Hide;
        Hide();
    }

    void Update()
    {
        if (!isFollowingMouse) return;
        UpdatePosition(Input.mousePosition);
    }

    void OnDestroy()
    {
        GameEvents.OnSlotHovered -= ShowForSlot;
        GameEvents.OnSlotHoverExit -= Hide;
        GameEvents.OnInventoryClosed -= Hide;
    }

    void ShowForSlot(int slotIndex)
    {
        var inventory = FindFirstObjectByType<Inventory>();
        var slot = inventory.slots[slotIndex];
        if (slot.item == null) return;

        Show(slot.item, slot.count);
    }

    public void Show(ItemData item, int count)
    {
        nameText.text = item.itemName;
        nameText.color = ItemRarityColor.Get(item.rarity);

        countText.text = count > 1 ? $"x{count}" : "";
        descriptionText.text = item.description;

        // TODO: Change the look of the tooltip based on the rarity
        // background.color = ItemRarityColor.Get(item.rarity) * new Color(1, 1, 1, 0.15f);

        isFollowingMouse = true;
        UpdatePosition(Input.mousePosition);

        canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        isFollowingMouse = false;
        canvasGroup.alpha = 0;
    }

    void UpdatePosition(Vector2 mousePos)
    {
        Vector2 offset = new Vector2(16, -16);
        Vector2 pos = mousePos + offset;

        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRT = canvas.transform as RectTransform;

        RectTransform rt = transform as RectTransform;
        Vector2 size = rt.sizeDelta;

        // Right overflow
        if (pos.x + size.x > Screen.width)
            pos.x = mousePos.x - size.x - offset.x;

        // Bottom overflow
        if (pos.y - size.y < 0)
            pos.y = mousePos.y + size.y + offset.y;

        rt.position = pos;
    }
}