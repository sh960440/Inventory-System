using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the inventory UI, including building the grid, updating category chips, and opening or closing the panel.
/// </summary>
public class InventoryUIController : MonoBehaviour
{
    /// <summary>
    /// Optional overrides for wiring when the controller is configured from code.
    /// </summary>
    public struct InventoryUIWiring
    {
        public RectTransform panel;
        public CanvasGroup canvasGroup;
        public InventorySlotUI slotPrefab;
        public Transform container;
        public GridLayoutGroup gridLayout;

        public Transform categoryButtonContainer;
        public InventoryCategoryButton categoryButtonPrefab;

        public DraggableItemUI dragUI;
    }

    [Header("Core")]
    [SerializeField] private Inventory inventory;

    [Header("Panel")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Grid")]
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private GridLayoutGroup gridLayout;

    [Header("Category filter")]
    [SerializeField] private Transform categoryButtonContainer;
    [SerializeField] private InventoryCategoryButton categoryButtonPrefab;

    [Header("Drag icon")]
    [SerializeField] private DraggableItemUI dragUI;

    private InventorySlotUI[] _slotsUI;
    private bool _useFadeAnimation;
    private float _fadeDuration;
    private Coroutine _currentAnim;

    // Reuse collections to avoid per-refresh allocations.
    private readonly HashSet<int> _visibleSet = new HashSet<int>();
    private readonly List<int> _visibleIndices = new List<int>();

    public IReadOnlyList<InventorySlotUI> SlotsUI => _slotsUI;
    public Inventory Inventory => inventory;

    private void OnEnable()
    {
        InventoryEvents.InventoryChanged += RefreshAll;
        InventoryEvents.EquipmentChanged += RefreshAll;

        InventoryEvents.InventoryToggleRequested += HandleOpen;
        InventoryEvents.InventoryCloseRequested += HandleClose;
    }

    private void OnDisable()
    {
        InventoryEvents.InventoryChanged -= RefreshAll;
        InventoryEvents.EquipmentChanged -= RefreshAll;

        InventoryEvents.InventoryToggleRequested -= HandleOpen;
        InventoryEvents.InventoryCloseRequested -= HandleClose;
    }

    /// <summary>
    /// Injects UI wiring dependencies in a single place. Does not change behavior when fields are already assigned in Inspector.
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

        if (wiring.dragUI != null) dragUI = wiring.dragUI;
    }

    /// <summary>
    /// Applies the inventory UI configuration and initializes the grid, category buttons, and drag settings.
    /// </summary>
    public void ApplyConfig(ItemSystemConfiguration config, IEquippedItemLookup equippedItemLookup, SlotHoverService slotHoverService = null)
    {
        if (!ValidateWiring())
            return;

        BuildInventoryUI(config.InventoryColumns, equippedItemLookup, slotHoverService);
        BuildCategoryButtons(config.CategoryButtons);

        _useFadeAnimation = config.UseFadeAnimation;
        _fadeDuration = config.FadeDuration;

        if (dragUI != null)
            dragUI.SetPointerOffset(config.DragIconOffset);

        RefreshAll();
        canvasGroup.alpha = 0;
        panel.gameObject.SetActive(false);
    }

    private bool ValidateWiring()
    {
        if (panel == null)
        {
            Debug.LogWarning("Missing required dependency: panel.", this);
            return false;
        }
        if (canvasGroup == null)
        {
            Debug.LogWarning("Missing required dependency: canvasGroup.", this);
            return false;
        }
        if (slotPrefab == null)
        {
            Debug.LogWarning("Missing required dependency: slotPrefab.", this);
            return false;
        }
        if (container == null)
        {
            Debug.LogWarning("Missing required dependency: container.", this);
            return false;
        }
        if (gridLayout == null)
        {
            Debug.LogWarning("Missing required dependency: gridLayout.", this);
            return false;
        }

        return true;
    }

    private void BuildInventoryUI(int inventoryColumns, IEquippedItemLookup equippedItemLookup, SlotHoverService slotHoverService = null)
    {
        IInventoryReadOnly model = inventory;
        var prefabRect = slotPrefab.GetComponent<RectTransform>();
        gridLayout.cellSize = new Vector2(prefabRect.rect.width, prefabRect.rect.height);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = inventoryColumns;
        _slotsUI = new InventorySlotUI[model.SlotCount];

        for (int i = 0; i < model.SlotCount; i++)
        {
            var ui = Instantiate(slotPrefab, container);
            ui.Setup(model, equippedItemLookup, i, slotHoverService, dragUI);
            _slotsUI[i] = ui;
        }
    }

    private void BuildCategoryButtons(IReadOnlyList<CategoryButtonConfig> configs)
    {
        if (categoryButtonContainer == null || categoryButtonPrefab == null || configs == null || configs.Count <= 0)
            return;
        foreach (var config in configs)
        {
            var button = Instantiate(categoryButtonPrefab, categoryButtonContainer);
            button.Initialize(inventory, config.Categories, config.Label);
        }
    }

    private void HandleOpen(bool open)
    {
        if (open)
            Open();
    }


    private void HandleClose()
    {
        Close();
    }

    private void Open()
    {
        panel.gameObject.SetActive(true);
        RefreshAll();

        if (!_useFadeAnimation)
        {
            canvasGroup.alpha = 1f;
            var pos = panel.anchoredPosition;
            pos.x = 0;
            panel.anchoredPosition = pos;
            return;
        }

        if (_currentAnim != null)
            StopCoroutine(_currentAnim);

        _currentAnim = StartCoroutine(AnimatePanel(0, 1f, _fadeDuration));
    }

    private void Close()
    {
        if (!_useFadeAnimation)
        {
            canvasGroup.alpha = 0f;
            panel.gameObject.SetActive(false);
            return;
        }

        if (_currentAnim != null)
            StopCoroutine(_currentAnim);

        _currentAnim = StartCoroutine(CloseRoutine());
    }

    private IEnumerator CloseRoutine()
    {
        yield return AnimatePanel(400, 0f, _fadeDuration);
        panel.gameObject.SetActive(false);
    }

    private void RefreshAll()
    {
        IInventoryReadOnly model = inventory;
        _visibleSet.Clear();
        model.GetFilteredSlotIndices(_visibleIndices);
        for (int i = 0; i < _visibleIndices.Count; i++)
            _visibleSet.Add(_visibleIndices[i]);

        for (int i = 0; i < _slotsUI.Length; i++)
        {
            bool visible = _visibleSet.Contains(i);
            _slotsUI[i].SetVisible(visible);

            if (visible)
                _slotsUI[i].Refresh();
        }
    }

    private IEnumerator AnimatePanel(float targetX, float targetAlpha, float duration)
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