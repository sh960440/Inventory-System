using UnityEngine;

/// <summary>
/// Maps ItemRarity values to UI display colors.
/// </summary>
public static class ItemRarityColor
{
    /// <summary>
    /// Returns a color for the given rarity.
    /// </summary>
    public static Color Get(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Uncommon:
                return new Color(0.3f, 0.9f, 0.3f);
            case ItemRarity.Rare:
                return new Color(0.3f, 0.6f, 1f);
            case ItemRarity.Epic:
                return new Color(0.7f, 0.4f, 1f);
            case ItemRarity.Legendary:
                return new Color(1f, 0.6f, 0.1f);
            default:
                return Color.white;
        }
    }
}