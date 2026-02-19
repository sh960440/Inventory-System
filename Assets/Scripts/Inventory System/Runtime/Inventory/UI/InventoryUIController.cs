using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    public RectTransform panel;
    public CanvasGroup canvasGroup;
    public Inventory inventory;
    public InventorySlotUI[] slotsUI;

    bool isOpen;
    Coroutine currentAnim;
    void Start()
    {
        for (int i = 0; i < slotsUI.Length; i++)
        {
            slotsUI[i].Setup(inventory, i);
        }
        
        canvasGroup.alpha = 0;
        panel.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        InventoryEvents.OnInventoryChanged += RefreshAll;
        InventoryEvents.OnEquipmentChanged += RefreshAll;
        RefreshAll();
    }

    void OnDisable()
    {
        InventoryEvents.OnInventoryChanged -= RefreshAll;
        InventoryEvents.OnEquipmentChanged -= RefreshAll;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool open = !isOpen;
            InventoryEvents.OnInventoryToggle?.Invoke(open);

            if (!open)
                InventoryEvents.OnInventoryClosed?.Invoke();

            if (isOpen) Close();
            else Open();
        }
    }

    void Open()
    {
        isOpen = true;
        panel.gameObject.SetActive(true);

        RefreshAll();

        if (currentAnim != null)
            StopCoroutine(currentAnim);

        currentAnim = StartCoroutine(AnimatePanel(0, 1f, 0.25f));
    }

    void Close()
    {
        isOpen = false;

        if (currentAnim != null)
            StopCoroutine(currentAnim);

        currentAnim = StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        yield return AnimatePanel(400, 0f, 0.25f);
        panel.gameObject.SetActive(false);
    }

    void RefreshAll()
    {
        var visibleSet = new HashSet<int>(inventory.GetFilteredSlotIndices());

        for (int i = 0; i < slotsUI.Length; i++)
        {
            bool visible = visibleSet.Contains(i);
            slotsUI[i].SetVisible(visible);

            if (visible)
                slotsUI[i].Refresh();
        }
    }

    IEnumerator AnimatePanel(float targetX, float targetAlpha, float duration)
    {
        float time = 0f;

        float startX = panel.anchoredPosition.x;
        float startAlpha = canvasGroup.alpha;

        Vector2 pos = panel.anchoredPosition;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            pos.x = Mathf.Lerp(startX, targetX, t);
            panel.anchoredPosition = pos;

            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        pos.x = targetX;
        panel.anchoredPosition = pos;
        canvasGroup.alpha = targetAlpha;
    }


}
