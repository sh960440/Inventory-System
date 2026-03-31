using UnityEngine;

/// <summary>
/// Defines one inventory category button: display text and which categories it includes.
/// </summary>
[System.Serializable]
public class CategoryButtonConfig
{
    [SerializeField] private string label;
    [SerializeField] private ItemCategory[] categories;

    public string Label => label;
    public ItemCategory[] Categories => categories;
}