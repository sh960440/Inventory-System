# Reusable Inventory System

Reusable inventory module with a small 3D demo:

- Inventory
- Equipment
- Hotbar
- Drag-and-drop UI
- Tooltips
- Context menu
- Filters/sort
- JSON save/load

## Requirements

- Unity 6 (project saved with `6000.3.9f1`)

## How to run the demo scene

1. Open `Assets/Scenes/Demo.unity` and Press Play.
2. Controls:
   - Character movement: W / A / S / D or arrow keys
   - Jump: Space
   - Rotate camera: Right-click
   - Zoom in/out: Mouse wheel
   - Pick up items: E
   - Toggle inventory: I
   - Split stack: X
   - Open context menu: Middle-click

## How to set up a new scene

Wire the following components in the Inspector:

| Role | Components |
|------|------------|
| Items | `ItemDatabase` — assign `ItemData` / `EquipmentData` / `ConsumableData` assets. |
| Gameplay | `Inventory`, `Equipment`, `Hotbar`. |
| UI | `InventoryUIController`, `EquipmentUIController`, `HotbarUIController` as needed; `ContextMenuUI`, `TooltipUI`, drag icon, slot prefabs. |
| Wiring | `ItemSystemConfigurator` — drag references above, assign a `ItemSystemConfiguration` asset. |
| Save | `SaveSystem` — references to `Inventory`, `Equipment`, `Hotbar`, and an `IItemDatabase` provider. |

## Code layout

- `Assets/Scripts/InventorySystem/Runtime/` — inventory, equipment, hotbar, UI, save DTOs, events.  
- `Assets/Scripts/Demo/` — player, camera, spawners, demo-only helpers.

## Class Diagrams
- Core Domain
```mermaid
classDiagram
  direction TB

  class InventoryEvents {
    <<static>>
  }

  class IInventoryReadOnly {
    <<interface>>
  }
  class IEquippedItemLookup {
    <<interface>>
  }
  class IItemUseHandler {
    <<interface>>
  }

  class ItemUseContext {
    <<struct>>
  }

  class Inventory {
    <<MonoBehaviour>>
  }
  class Equipment {
    <<MonoBehaviour>>
  }
  class Hotbar {
    <<MonoBehaviour>>
  }

  class InventorySlot
  class HotbarSlot
  class InventoryFilterState

  class InventoryUseHandlerRegistry
  class ConsumableUseHandler
  class EquipmentUseHandler

  class ItemData {
    <<ScriptableObject>>
  }
  class EquipmentData
  class ConsumableData

  IInventoryReadOnly <|.. Inventory
  IEquippedItemLookup <|.. Equipment
  IItemUseHandler <|.. ConsumableUseHandler
  IItemUseHandler <|.. EquipmentUseHandler

  Inventory *-- InventorySlot
  Inventory *-- InventoryFilterState
  Inventory *-- InventoryUseHandlerRegistry
  InventoryUseHandlerRegistry o-- IItemUseHandler

  Hotbar *-- HotbarSlot
  HotbarSlot --> Inventory
  HotbarSlot --> ItemData

  InventorySlot --> ItemData
  Equipment --> Inventory

  ItemData <|-- EquipmentData
  ItemData <|-- ConsumableData

  Inventory ..> InventoryEvents
  Equipment ..> InventoryEvents
  Hotbar ..> InventoryEvents
  IItemUseHandler ..> ItemUseContext
```
- UI
```mermaid
classDiagram
  direction TB

  class InventoryEvents {
    <<static>>
  }

  class UISlotBase {
    <<abstract>>
    <<MonoBehaviour>>
  }
  class InventorySlotUI
  class EquipmentSlotUI
  class HotbarSlotUI

  class InventoryUIController {
    <<MonoBehaviour>>
  }
  class EquipmentUIController {
    <<MonoBehaviour>>
  }
  class HotbarUIController {
    <<MonoBehaviour>>
  }

  class SlotHoverService {
    <<MonoBehaviour>>
  }
  class DraggableItemUI {
    <<MonoBehaviour>>
  }
  class ContextMenuUI {
    <<MonoBehaviour>>
  }
  class TooltipUI {
    <<MonoBehaviour>>
  }

  UISlotBase <|-- InventorySlotUI
  UISlotBase <|-- EquipmentSlotUI
  UISlotBase <|-- HotbarSlotUI

  InventoryUIController ..> InventorySlotUI
  InventoryUIController ..> SlotHoverService
  InventoryUIController ..> DraggableItemUI
  EquipmentUIController ..> EquipmentSlotUI
  HotbarUIController ..> HotbarSlotUI

  ContextMenuUI ..> InventoryEvents
  TooltipUI ..> InventoryEvents
  InventorySlotUI ..> InventoryEvents
  HotbarSlotUI ..> InventoryEvents
```
- Save / Load
```mermaid
classDiagram
  direction TB

  class Inventory {
    <<MonoBehaviour>>
  }
  class Hotbar {
    <<MonoBehaviour>>
  }
  class Equipment {
    <<MonoBehaviour>>
  }

  class SaveSystem {
    <<MonoBehaviour>>
    +Save() Load()
  }
  class SaveData {
    <<serializable>>
  }
  class InventorySaveData
  class HotbarSaveData
  class EquipmentSaveData
  class InventorySlotSaveData
  class HotbarSlotSaveData
  class EquipmentSlotSaveData

  SaveData *-- InventorySaveData
  SaveData *-- HotbarSaveData
  SaveData *-- EquipmentSaveData
  InventorySaveData *-- InventorySlotSaveData
  HotbarSaveData *-- HotbarSlotSaveData
  EquipmentSaveData *-- EquipmentSlotSaveData

  SaveSystem ..> Inventory
  SaveSystem ..> Hotbar
  SaveSystem ..> Equipment
  SaveSystem ..> SaveData : JSON file
```
