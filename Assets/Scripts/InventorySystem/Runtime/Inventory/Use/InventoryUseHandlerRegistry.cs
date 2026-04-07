using System.Collections.Generic;

/// <summary>
/// Maintains an ordered list of item-use handlers and executes the first one that can handle the item.
/// </summary>
public sealed class InventoryUseHandlerRegistry
{
    private readonly List<IItemUseHandler> _handlers = new List<IItemUseHandler>(2);

    /// <summary>
    /// Removes all handlers.
    /// </summary>
    public void Clear()
    {
        _handlers.Clear();
    }

    /// <summary>
    /// Appends a handler; earlier entries are preferred when multiple could apply.
    /// </summary>
    public void Register(IItemUseHandler handler)
    {
        if (handler == null)
            return;
        _handlers.Add(handler);
    }

    /// <summary>
    /// Ensures default handlers are present when none were registered.
    /// </summary>
    public void EnsureDefaults()
    {
        if (_handlers.Count > 0)
            return;

        _handlers.Add(new ConsumableUseHandler());
        _handlers.Add(new EquipmentUseHandler());
    }

    /// <summary>
    /// Attempts to use the given slot item via the first matching handler. Returns true if a handler was found and executed.
    /// </summary>
    public bool TryUse(ItemUseContext ctx, InventorySlot slot)
    {
        if (slot == null || slot.Item == null)
            return false;

        EnsureDefaults();

        var item = slot.Item;
        for (int i = 0; i < _handlers.Count; i++)
        {
            var h = _handlers[i];
            if (h != null && h.CanUse(item))
            {
                h.Use(ctx, slot);
                return true;
            }
        }

        return false;
    }
}