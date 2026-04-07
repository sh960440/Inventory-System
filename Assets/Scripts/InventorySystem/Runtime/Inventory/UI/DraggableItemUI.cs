using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Floating item icon that follows the pointer during drag operations.
/// </summary>
public class DraggableItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Vector2 _pointerOffset = new Vector2(18f, -18f);

    /// <summary>
    /// Active drag payload, or null when not dragging.
    /// </summary>
    public DragItemContext? CurrentContext { get; private set; }

    private void Awake()
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Sets the pointer offset used when dragging items.
    /// </summary>
    public void SetPointerOffset(Vector2 screenSpaceOffset)
    {
        _pointerOffset = screenSpaceOffset;
    }

    /// <summary>
    /// Shows the drag icon and stores drag item context for drop handling.
    /// </summary>
    public void BeginDrag(DragItemContext ctx, Sprite sprite)
    {
        if (icon == null || rect == null)
            return;

        CurrentContext = ctx;

        icon.sprite = sprite;
        icon.preserveAspect = true;
        icon.color = new Color(1f, 1f, 1f, 0.7f);

        rect.localScale = Vector3.one * 1.1f;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Moves the drag icon to the mouse position with a small offset.
    /// </summary>
    public void FollowMouse()
    {
        if (!gameObject.activeSelf || Mouse.current == null || rect == null)
            return;

        rect.position = Mouse.current.position.ReadValue() + _pointerOffset;
    }

    /// <summary>
    /// Hides the drag icon and clears context.
    /// </summary>
    public void EndDrag()
    {
        CurrentContext = null;

        if (icon != null)
        {
            icon.sprite = null;
            icon.color = Color.white;
        }

        if (rect != null)
            rect.localScale = Vector3.one;

        gameObject.SetActive(false);
    }
}