using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public RectTransform panel;
    public CanvasGroup canvasGroup;
    public Inventory inventory;
    public InventorySlotUI slotPrefab;
    public InventorySlotUI[] slotsUI;
    public Transform container;
    public GridLayoutGroup gridLayout;

    public Transform categoryButtonContainer;
    public InventoryCategoryButton categoryButtonPrefab;
    public ContextMenuUI contextMenuUI;
    public DraggableItemUI dragUI;
    public TooltipUI tooltipUI;

    private bool useFadeAnimation;
    private float fadeDuration;

    //bool isOpen;
    Coroutine currentAnim;

    void OnEnable()
    {
        InventoryEvents.InventoryChanged += RefreshAll;
        InventoryEvents.EquipmentChanged += RefreshAll;

        InventoryEvents.InventoryToggled += HandleOpen;
        InventoryEvents.InventoryClosed += HandleClose;
    }

    void OnDisable()
    {
        InventoryEvents.InventoryChanged -= RefreshAll;
        InventoryEvents.EquipmentChanged -= RefreshAll;

        InventoryEvents.InventoryToggled -= HandleOpen;
        InventoryEvents.InventoryClosed -= HandleClose;
    }

    public void ApplyConfig(ItemSystemConfiguration config)
    {
        BuildInventoryUI(config.inventoryColumns);
        BuildCategoryButtons(config.categoryButtons);

        useFadeAnimation = config.useFadeAnimation;
        fadeDuration = config.fadeDuration;

        RefreshAll();
        canvasGroup.alpha = 0;
        panel.gameObject.SetActive(false);
    }

    void BuildInventoryUI(int inventoryColumns)
    {
        gridLayout.cellSize = new Vector2(slotPrefab.GetComponent<RectTransform>().rect.width, slotPrefab.GetComponent<RectTransform>().rect.height);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = inventoryColumns;
        slotsUI = new InventorySlotUI[inventory.SlotCount];

        for (int i = 0; i < inventory.SlotCount; i++)
        {
            var ui = Instantiate(slotPrefab, container);
            ui.slotIndex = i;
            ui.contextMenuUI = contextMenuUI;
            ui.dragUI = dragUI;
            ui.tooltipUI = tooltipUI;
            slotsUI[i] = ui;
            slotsUI[i].Setup(inventory, i);
        }
    }

    void BuildCategoryButtons(List<CategoryButtonConfig> configs)
    {
        if (categoryButtonContainer == null || categoryButtonPrefab == null || configs.Count <= 0) return;
        foreach (var config in configs)
        {
            var button = Instantiate(categoryButtonPrefab, categoryButtonContainer);
            button.Initialize(inventory, config.categories, config.label);
        }
    }

    void HandleOpen(bool open)
    {
        if (open) Open();
    }

    void HandleClose()
    {
        Close();
    }

    void Open()
    {
        panel.gameObject.SetActive(true);
        RefreshAll();

        if (!useFadeAnimation)
        {
            canvasGroup.alpha = 1f;
            var pos = panel.anchoredPosition;
            pos.x = 0;
            panel.anchoredPosition = pos;
            return;
        }

        if (currentAnim != null)
            StopCoroutine(currentAnim);

        currentAnim = StartCoroutine(AnimatePanel(0, 1f, fadeDuration));
    }

    void Close()
    {
        if (!useFadeAnimation)
        {
            canvasGroup.alpha = 0f;
            panel.gameObject.SetActive(false);
            return;
        }

        if (currentAnim != null)
            StopCoroutine(currentAnim);

        currentAnim = StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        yield return AnimatePanel(400, 0f, fadeDuration);
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
