using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;
    public bool stackable = true;
    public int maxStack = 99;
}
