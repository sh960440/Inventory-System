using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    public RectTransform panel;
    public CanvasGroup canvasGroup;

    bool isOpen;

    void Start()
    {
        canvasGroup.alpha = 0;
        panel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool open = !panel.gameObject.activeSelf;
            //panel.gameObject.SetActive(open);

            GameEvents.OnInventoryToggle?.Invoke(open);

            if (!open)
            {
                GameEvents.OnInventoryClosed?.Invoke();
            }

            if (isOpen) Close();
            else Open();
        }
    }

    void Open()
    {
        isOpen = true;
        panel.gameObject.SetActive(true);

        LeanTween.moveX(panel, 0, 0.25f).setEaseOutCubic();
        LeanTween.alphaCanvas(canvasGroup, 1, 0.2f);
    }

    void Close()
    {
        isOpen = false;

        LeanTween.moveX(panel, 400, 0.25f).setEaseInCubic()
            .setOnComplete(() => panel.gameObject.SetActive(false));
        LeanTween.alphaCanvas(canvasGroup, 0, 0.2f);
    }
}
