using UnityEngine;
using UnityEngine.UI;

public class DraggableItemUI : MonoBehaviour
{
    public Image icon;
    public RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        //Hide();
    }

    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
        icon.preserveAspect = true;
        gameObject.SetActive(true);
    }

    public void FollowMouse()
    {
        rect.position = Input.mousePosition;
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
