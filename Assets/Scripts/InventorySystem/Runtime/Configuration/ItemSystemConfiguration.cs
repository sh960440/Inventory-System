using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemSystemConfig", menuName = "Config/Inventory Config")]
public class ItemSystemConfiguration : ScriptableObject
{
    [Header("Inventory Layout")]
    public int inventoryRows;
    public int inventoryColumns;

    [Header("Catrgory Buttons")]
    public List<CategoryButtonConfig> categoryButtons;

    [Header("Hotbar")]
    [Range(1, 9)]
    public int hotkeyCount;

    [Header("Stack")]
    public bool allowStacking = true;
    public bool allowSplitStack = true;

    [Header("Interaction")]
    public bool allowInventoryDoubleClickUse = true;
    public bool allowHotbarDoubleClickUse = true;

    [Header("UI Behavior")]
    //public bool pauseGameWhenOpen = true;
    public bool useFadeAnimation = true;
    public float fadeDuration = 0.25f;
}