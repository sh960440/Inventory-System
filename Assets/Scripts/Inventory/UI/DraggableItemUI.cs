using UnityEngine;
using UnityEngine.UI;

public class DraggableItemUI : MonoBehaviour
{
    public Image icon;
    public RectTransform rect;

    Vector2 offset = new Vector2(18, -18);

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
        icon.preserveAspect = true;
        icon.color = new Color(1f, 1f, 1f, 0.7f);
        rect.localScale = Vector3.one * 1.1f;
        gameObject.SetActive(true);
    }

    public void FollowMouse()
    {
        rect.position = (Vector2)Input.mousePosition + offset;
    }
    
    public void Hide()
    {
        icon.sprite = null;
        icon.color = Color.white;
        rect.localScale = Vector3.one;

        gameObject.SetActive(false);
    }
}
