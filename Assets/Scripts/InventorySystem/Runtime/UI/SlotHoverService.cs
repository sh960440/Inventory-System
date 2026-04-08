using UnityEngine;

/// <summary>
/// Tracks which inventory slot is currently hovered.
/// </summary>
public class SlotHoverService : MonoBehaviour
{
    /// <summary>
    /// Hovered grid index, or -1 when none.
    /// </summary>
    public int CurrentHoveredIndex { get; private set; } = -1;

    /// <summary>
    /// Sets the currently hovered slot index.
    /// </summary>
    public void SetHovered(int index)
    {
        CurrentHoveredIndex = index;
    }

    /// <summary>
    /// Clears hover state.
    /// </summary>
    public void ClearHovered()
    {
        CurrentHoveredIndex = -1;
    }
}