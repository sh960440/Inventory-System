using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UISlotBase : MonoBehaviour, IPointerClickHandler
{
    [Header("Double Click")]
    [SerializeField] float doubleClickThreshold = 0.25f;

    float lastClickTime;

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
        }
    }

    void HandleLeftClick()
    {
        if (Time.unscaledTime - lastClickTime <= doubleClickThreshold)
        {
            lastClickTime = 0;
            OnDoubleClick();
        }
        else
        {
            lastClickTime = Time.unscaledTime;
        }
    }

    /// <summary>
    /// Double left click behavior (must implement)
    /// </summary>
    protected abstract void OnDoubleClick();

    /// <summary>
    /// Right click behavior (optional)
    /// </summary>
    protected virtual void OnRightClick(PointerEventData eventData) { }
}
