using UnityEngine;
using UnityEngine.UI;

public class DraggableItemUI : MonoBehaviour
{
    public Image icon;
    public RectTransform rect;

    Vector2 offset = new Vector2(18, -18);

    public DragItemContext? CurrentContext { get; private set; }

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        //gameObject.SetActive(false);
    }

    public void BeginDrag(DragItemContext ctx, Sprite sprite)
    {
        CurrentContext = ctx;

        icon.sprite = sprite;
        icon.preserveAspect = true;
        icon.color = new Color(1f, 1f, 1f, 0.7f);

        rect.localScale = Vector3.one * 1.1f;
        gameObject.SetActive(true);
    }

    public void FollowMouse()
    {
        if (!gameObject.activeSelf) return;

        rect.position = (Vector2)Input.mousePosition + offset;
    }

    public void EndDrag()
    {
        CurrentContext = null;

        icon.sprite = null;
        icon.color = Color.white;
        rect.localScale = Vector3.one;

        gameObject.SetActive(false);
    }
}
