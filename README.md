## Quick start (Scene setup)

- **Required objects**
  - **Item database**: add `ItemDatabase` to the scene and populate its `items` list.
  - **Core systems**: add `Inventory`, `Equipment`, `Hotbar` (optional) to the scene.
  - **UI**: add `InventoryUIController`, `ContextMenuUI`, `TooltipUI` (and Hotbar UI if used).
  - **Configurator**: add `ItemSystemConfigurator` and assign references + `ItemSystemConfiguration`.