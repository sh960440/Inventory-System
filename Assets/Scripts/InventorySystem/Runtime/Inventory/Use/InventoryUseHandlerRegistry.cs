using System.Collections.Generic;

/// SRP helper: manages item use handlers and selects a handler for an item.
public sealed class InventoryUseHandlerRegistry
{
    readonly List<IItemUseHandler> handlers = new List<IItemUseHandler>(2);

    public void Clear()
    {
        handlers.Clear();
    }

    public void Register(IItemUseHandler handler)
    {
        if (handler == null) return;
        handlers.Add(handler);
    }

    /// <summary>
    /// Ensures default handlers are present when none were registered.
    /// </summary>
    public void EnsureDefaults()
    {
        if (handlers.Count > 0)
            return;

        handlers.Add(new ConsumableUseHandler());
        handlers.Add(new EquipmentUseHandler());
    }

    /// <summary>
    /// Attempts to use the given slot item via the first matching handler.
    /// </summary>
    /// <returns>True if a handler was found and executed.</returns>
    public bool TryUse(ItemUseContext ctx, InventorySlot slot)
    {
        if (slot == null || slot.item == null)
            return false;

        EnsureDefaults();

        var item = slot.item;
        for (int i = 0; i < handlers.Count; i++)
        {
            var h = handlers[i];
            if (h != null && h.CanUse(item))
            {
                h.Use(ctx, slot);
                return true;
            }
        }

        return false;
    }
}