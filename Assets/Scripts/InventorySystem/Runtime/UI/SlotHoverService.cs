using UnityEngine;

/// <summary>
/// Tracks which inventory slot is currently hovered. Used for split-stack input.
/// </summary>
public class SlotHoverService : MonoBehaviour
{
    public int CurrentHoveredIndex { get; private set; } = -1;

    public void SetHovered(int index)
    {
        CurrentHoveredIndex = index;
    }

    public void ClearHovered()
    {
        CurrentHoveredIndex = -1;
    }
}