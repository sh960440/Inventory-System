using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that holds settings for the inventory system.
/// </summary>
[CreateAssetMenu(fileName = "ItemSystemConfig", menuName = "Config/Inventory Config")]
public class ItemSystemConfiguration : ScriptableObject
{
    [Header("Inventory Layout")]
    [SerializeField] private int inventoryRows;
    [SerializeField] private int inventoryColumns;

    [Header("Category Buttons")]
    [SerializeField] private List<CategoryButtonConfig> categoryButtons = new List<CategoryButtonConfig>();

    [Header("Hotbar")]
    [Range(1, 9)]
    [SerializeField] private int hotkeyCount;

    [Header("Stack")]
    [SerializeField] private bool allowStacking = true;
    [SerializeField] private bool allowSplitStack = true;

    [Header("Interaction")]
    [SerializeField] private bool allowInventoryDoubleClickUse = true;
    [SerializeField] private bool allowHotbarDoubleClickUse = true;

    [Header("UI Behavior")]
    //[SerializeField] private public bool pauseGameWhenOpen = true; // TBD
    [SerializeField] private bool useFadeAnimation = true;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private Vector2 dragIconOffset = new Vector2(18f, -18f);

    public int InventoryRows => inventoryRows;
    public int InventoryColumns => inventoryColumns;
    public int InventorySlotCount => InventoryRows * InventoryColumns;
    public IReadOnlyList<CategoryButtonConfig> CategoryButtons => categoryButtons;
    public int HotkeyCount => hotkeyCount;
    public bool AllowStacking => allowStacking;
    public bool AllowSplitStack => allowSplitStack;
    public bool AllowInventoryDoubleClickUse => allowInventoryDoubleClickUse;
    public bool AllowHotbarDoubleClickUse => allowHotbarDoubleClickUse;
    public bool UseFadeAnimation => useFadeAnimation;
    public float FadeDuration => fadeDuration;
    public Vector2 DragIconOffset => dragIconOffset;
}