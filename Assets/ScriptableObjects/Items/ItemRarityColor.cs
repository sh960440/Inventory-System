using UnityEngine;

public static class ItemRarityColor
{
    public static Color Get(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Uncommon:
                return new Color(0.3f, 0.9f, 0.3f);   // Green
            case ItemRarity.Rare:
                return new Color(0.3f, 0.6f, 1f);     // Blue
            case ItemRarity.Epic:
                return new Color(0.7f, 0.4f, 1f);     // Purple
            case ItemRarity.Legendary:
                return new Color(1f, 0.6f, 0.1f);     // Orange
            default:
                return Color.white;                   // Common
        }
    }
}