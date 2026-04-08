using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statsText;

    [Header("Panel")]
    [SerializeField] private CanvasGroup canvasGroup;

    private bool _followMouse;
    private readonly Vector2 _screenOffset = new Vector2(16f, -16f);

    private void OnEnable()
    {
        InventoryEvents.TooltipRequested += Show;
        InventoryEvents.TooltipHidden += Hide;
        InventoryEvents.InventoryCloseRequested += Hide;
        InputSystem.onEvent += OnInputEvent;
        Hide();
    }

    private void OnDisable()
    {
        InventoryEvents.TooltipRequested -= Show;
        InventoryEvents.TooltipHidden -= Hide;
        InventoryEvents.InventoryCloseRequested -= Hide;
        InputSystem.onEvent -= OnInputEvent;
    }

    /// <summary>
    /// Hides the tooltip and stops following the pointer.
    /// </summary>
    public void Hide()
    {
        _followMouse = false;
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (!_followMouse)
            return;
        if (Mouse.current == null || device != Mouse.current)
            return;
        if (!eventPtr.IsA<StateEvent>())
            return;

        UpdatePosition(Mouse.current.position.ReadValue());
    }

    private void Show(ItemUIContext ctx)
    {
        if (ctx.Item == null || canvasGroup == null)
            return;

        if (nameText != null)
        {
            nameText.text = ctx.Item.ItemName;
            nameText.color = ItemRarityColor.Get(ctx.Item.Rarity);
        }

        if (countText != null)
            countText.text = ctx.StackCount > 1 ? $"x{ctx.StackCount}" : "";

        if (descriptionText != null)
            descriptionText.text = ctx.Item.Description;

        if (statsText != null)
        {
            statsText.text = "";
            statsText.gameObject.SetActive(false);
        }

        if (ctx.Item is EquipmentData eq && statsText != null)
        {
            statsText.gameObject.SetActive(true);
            statsText.text += $"{eq.EquipSlot}\n";

            foreach (var mod in eq.Modifiers)
            {
                statsText.text +=
                    mod.ModifierType == ModifierType.Percent
                    ? $"+{mod.Value}% {mod.StatType}\n"
                    : $"+{mod.Value} {mod.StatType}\n";
            }
        }

        canvasGroup.alpha = 1f;
        _followMouse = true;

        if (Mouse.current != null)
            UpdatePosition(Mouse.current.position.ReadValue());
    }

    private void UpdatePosition(Vector2 mousePos)
    {
        var rt = transform as RectTransform;
        if (rt == null)
            return;

        Vector2 pos = mousePos + _screenOffset;
        Vector2 size = rt.sizeDelta;

        if (pos.x + size.x > Screen.width)
            pos.x = mousePos.x - size.x - _screenOffset.x;

        if (pos.y - size.y < 0f)
            pos.y = mousePos.y + size.y + _screenOffset.y;

        rt.position = pos;
    }
}