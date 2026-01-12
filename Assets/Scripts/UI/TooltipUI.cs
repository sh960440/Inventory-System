using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text countText;
    public TMP_Text descriptionText;
    bool isFollowingMouse = false;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isFollowingMouse) return;
        UpdatePosition(Input.mousePosition);
    }

    public void Show(ItemData item, int count)
    {
        nameText.text = item.itemName;
        countText.text = count > 1 ? $"x{count}" : "";
        descriptionText.text = item.description;

        isFollowingMouse = true;
        UpdatePosition(Input.mousePosition);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        isFollowingMouse = false;
        gameObject.SetActive(false);
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