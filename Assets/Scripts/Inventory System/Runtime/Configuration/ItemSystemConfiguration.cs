using UnityEngine;

[CreateAssetMenu(fileName = "ItemSystemConfig", menuName = "Config/Inventory Config")]
public class ItemSystemConfiguration : ScriptableObject
{
    [Header("Inventory Layout")]
    public int inventoryRows = 5;
    public int inventoryColumns = 8;
    public Vector2 inventorySlotSize = new Vector2(64, 64);

    [Header("Hotbar")]
    [Range(1, 9)]
    public int hotkeyCount = 5;
    public Vector2 hotbarSlotSize = new Vector2(64, 64);

    [Header("Equipment")]
    public Vector2 equipmentSlotSize = new Vector2(64, 64);

    [Header("Stack")]
    public bool allowStacking = true;
    public bool allowSplitStack = true;

    [Header("Interaction")]
    public bool allowInventoryDoubleClickUse = true;
    public bool allowHotbarDoubleClickUse = false;
    public float doubleClickThreshold = 0.25f;

    [Header("UI Behavior")]
    public bool pauseGameWhenOpen = true;
    public bool useFadeAnimation = true;
    public float fadeDuration = 0.2f;
}