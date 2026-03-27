using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public struct InventoryUIWiring
    {
        public RectTransform panel;
        public CanvasGroup canvasGroup;
        public InventorySlotUI slotPrefab;
        public Transform container;
        public GridLayoutGroup gridLayout;

        public Transform categoryButtonContainer;
        public InventoryCategoryButton categoryButtonPrefab;

        public ContextMenuUI contextMenuUI;
        public DraggableItemUI dragUI;
        public TooltipUI tooltipUI;
    }

    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;
    public Inventory inventory;
    [SerializeField] private InventorySlotUI slotPrefab;
    public InventorySlotUI[] slotsUI;
    [SerializeField] private Transform container;
    [SerializeField] private GridLayoutGroup gridLayout;

    [SerializeField] private Transform categoryButtonContainer;
    [SerializeField] private InventoryCategoryButton categoryButtonPrefab;
    [SerializeField] private ContextMenuUI contextMenuUI;
    [SerializeField] private DraggableItemUI dragUI;
    [SerializeField] private TooltipUI tooltipUI;

    private bool useFadeAnimation;
    private float fadeDuration;

    //bool isOpen;
    Coroutine currentAnim;

    // Reuse collections to avoid per-refresh allocations.
    readonly HashSet<int> _visibleSet = new HashSet<int>();
    readonly List<int> _visibleIndices = new List<int>();

    /// <summary>
    /// Injects UI wiring dependencies in a single place.
    /// Does not change behavior when you already assigned fields in Inspector.
    /// </summary>
    public void ApplyWiring(InventoryUIWiring wiring)
    {
        if (wiring.panel != null) panel = wiring.panel;
        if (wiring.canvasGroup != null) canvasGroup = wiring.canvasGroup;
        if (wiring.slotPrefab != null) slotPrefab = wiring.slotPrefab;
        if (wiring.container != null) container = wiring.container;
        if (wiring.gridLayout != null) gridLayout = wiring.gridLayout;

        if (wiring.categoryButtonContainer != null) categoryButtonContainer = wiring.categoryButtonContainer;
        if (wiring.categoryButtonPrefab != null) categoryButtonPrefab = wiring.categoryButtonPrefab;

        if (wiring.contextMenuUI != null) contextMenuUI = wiring.contextMenuUI;
        if (wiring.dragUI != null) dragUI = wiring.dragUI;
        if (wiring.tooltipUI != null) tooltipUI = wiring.tooltipUI;
    }

    void OnEnable()
    {
        InventoryEvents.InventoryChanged += RefreshAll;
        InventoryEvents.EquipmentChanged += RefreshAll;

        InventoryEvents.InventoryToggleRequested += HandleOpen;
        InventoryEvents.InventoryCloseRequested += HandleClose;
    }

    void OnDisable()
    {
        InventoryEvents.InventoryChanged -= RefreshAll;
        InventoryEvents.EquipmentChanged -= RefreshAll;

        InventoryEvents.InventoryToggleRequested -= HandleOpen;
        InventoryEvents.InventoryCloseRequested -= HandleClose;
    }

    public void ApplyConfig(ItemSystemConfiguration config, IEquippedItemLookup equippedItemLookup, SlotHoverService slotHoverService = null)
    {
        if (!ValidateWiring()) return;

        BuildInventoryUI(config.inventoryColumns, equippedItemLookup, slotHoverService);
        BuildCategoryButtons(config.categoryButtons);

        useFadeAnimation = config.useFadeAnimation;
        fadeDuration = config.fadeDuration;

        RefreshAll();
        canvasGroup.alpha = 0;
        panel.gameObject.SetActive(false);
    }

    bool ValidateWiring()
    {
        if (panel == null)
        {
            Debug.LogWarning("[InventoryUIController] Missing required dependency: panel.", this);
            return false;
        }
        if (canvasGroup == null)
        {
            Debug.LogWarning("[InventoryUIController] Missing required dependency: canvasGroup.", this);
            return false;
        }
        if (slotPrefab == null)
        {
            Debug.LogWarning("[InventoryUIController] Missing required dependency: slotPrefab.", this);
            return false;
        }
        if (container == null)
        {
            Debug.LogWarning("[InventoryUIController] Missing required dependency: container.", this);
            return false;
        }
        if (gridLayout == null)
        {
            Debug.LogWarning("[InventoryUIController] Missing required dependency: gridLayout.", this);
            return false;
        }

        return true;
    }

    void BuildInventoryUI(int inventoryColumns, IEquippedItemLookup equippedItemLookup, SlotHoverService slotHoverService = null)
    {
        IInventoryReadOnly model = inventory;
        gridLayout.cellSize = new Vector2(slotPrefab.GetComponent<RectTransform>().rect.width, slotPrefab.GetComponent<RectTransform>().rect.height);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = inventoryColumns;
        slotsUI = new InventorySlotUI[model.SlotCount];

        for (int i = 0; i < model.SlotCount; i++)
        {
            var ui = Instantiate(slotPrefab, container);
            ui.slotIndex = i;
            ui.contextMenuUI = contextMenuUI;
            ui.dragUI = dragUI;
            ui.tooltipUI = tooltipUI;
            slotsUI[i] = ui;
            slotsUI[i].Setup(model, equippedItemLookup, i, slotHoverService);
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
        IInventoryReadOnly model = inventory;
        _visibleSet.Clear();
        model.GetFilteredSlotIndices(_visibleIndices);
        for (int i = 0; i < _visibleIndices.Count; i++)
            _visibleSet.Add(_visibleIndices[i]);

        for (int i = 0; i < slotsUI.Length; i++)
        {
            bool visible = _visibleSet.Contains(i);
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
