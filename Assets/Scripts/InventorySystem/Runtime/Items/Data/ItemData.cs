using UnityEngine;

/// <summary>
/// Defines the data for an inventory item, including its identity, visuals, and basic properties.
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;

    [Header("Display")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private string description;

    [Header("Classification")]
    [SerializeField] private ItemRarity rarity = ItemRarity.Common;
    [SerializeField] private ItemCategory category;

    [Header("Stacking")]
    [SerializeField] private bool stackable = true;
    [SerializeField] private int maxStack = 99;

    [Header("World")]
    [SerializeField] private GameObject worldPrefab;

    [Header("Flags")]
    [SerializeField] private bool consumable = false;

    public string Id => id;
    public string ItemName => itemName;
    public Sprite Icon => icon;
    public string Description => description;
    public ItemRarity Rarity => rarity;
    public ItemCategory Category => category;
    public bool Stackable => stackable;
    public int MaxStack => maxStack;
    public GameObject WorldPrefab => worldPrefab;
    public bool Consumable => consumable;
}