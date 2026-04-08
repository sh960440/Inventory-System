using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Provides shared click-handling behavior for UI slots, including left double-click and optional right or middle clicks.
/// </summary>
public abstract class UISlotBase : MonoBehaviour, IPointerClickHandler
{
    [Header("Click")]
    [SerializeField] private float doubleClickThreshold = 0.25f;

    private float _lastClickTime;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                HandleLeftClick();
                break;
            case PointerEventData.InputButton.Right:
                OnRightClick(eventData);
                break;
            case PointerEventData.InputButton.Middle:
                OnMiddleClick(eventData);
                break;
        }
    }

    private void HandleLeftClick()
    {
        if (Time.unscaledTime - _lastClickTime <= doubleClickThreshold)
        {
            _lastClickTime = 0;
            OnDoubleClick();
        }
        else
        {
            _lastClickTime = Time.unscaledTime;
        }
    }

    /// <summary>
    /// Double left click behavior
    /// </summary>
    protected abstract void OnDoubleClick();

    /// <summary>
    /// Right click behavior (optional - override when needed)
    /// </summary>
    protected virtual void OnRightClick(PointerEventData eventData) { }

    /// <summary>
    /// Middle click behavior (optional - override when needed)
    /// </summary>
    protected virtual void OnMiddleClick(PointerEventData eventData) { }
}