using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class TooltipUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text countText;
    public TMP_Text descriptionText;
    public TMP_Text statsText;
    public Image background;
    public CanvasGroup canvasGroup;

    bool isFollowingMouse;

    void OnEnable()
    {
        InventoryEvents.TooltipRequested += Show;
        InventoryEvents.TooltipHidden += Hide;
        InventoryEvents.InventoryClosed += Hide;
        Hide();
    }

    void OnDisable()
    {
        InventoryEvents.TooltipRequested -= Show;
        InventoryEvents.TooltipHidden -= Hide;
        InventoryEvents.InventoryClosed -= Hide;
    }

    void Update()
    {
        if (isFollowingMouse)
            UpdatePosition(Mouse.current.position.ReadValue());
    }

    void Show(ItemUIContext ctx)
    {
        if (ctx.item == null) return;

        nameText.text = ctx.item.itemName;
        nameText.color = ItemRarityColor.Get(ctx.item.rarity);

        countText.text = ctx.count > 1 ? $"x{ctx.count}" : "";
        descriptionText.text = ctx.item.description;

        statsText.text = "";
        statsText.gameObject.SetActive(false);

        if (ctx.item is EquipmentData eq)
        {
            statsText.gameObject.SetActive(true);
            statsText.text += $"{eq.equipSlot}\n";

            foreach (var mod in eq.modifiers)
            {
                statsText.text +=
                    mod.modifierType == ModifierType.Percent
                    ? $"+{mod.value}% {mod.statType}\n"
                    : $"+{mod.value} {mod.statType}\n";
            }
        }

        // TODO: Change the look of the tooltip based on the rarity
        // backgroundImage.color = ItemRarityColor.Get(item.rarity) * new Color(1, 1, 1, 0.15f);

        canvasGroup.alpha = 1;
        isFollowingMouse = true;
        UpdatePosition(Mouse.current.position.ReadValue());
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