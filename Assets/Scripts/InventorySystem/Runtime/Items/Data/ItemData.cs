using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Item ID")]
    [SerializeField] private string id;
    public string Id => id;

    public string itemName;
    public Sprite icon;
    public string description;

    public ItemRarity rarity = ItemRarity.Common;
    
    public bool stackable = true;
    public int maxStack = 99;
    public GameObject worldPrefab;
    public bool consumable = false;

    public ItemCategory category;
}