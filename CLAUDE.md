# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

GGJ2026Mask is a Unity 6 (6.0.3.6f1) 2D puzzle/adventure game created for Global Game Jam 2026. The player wears different masks that grant unique abilities to solve puzzles and interact with NPCs.

## Opening the Project

Open in Unity Hub and select Unity 6.0.3.6f1. No command-line build system - use Unity Editor for building and running.

## Architecture

### Singleton Pattern
Core managers use singletons accessible via `.Instance` or `.i`:
- `GameController.Instance` - Game state machine (FreeRoam, Menu, Paused, Shaman, ToughGuy)
- `PlayerController.Instance` - Player input and mask system
- `DialogManager.Instance` - Dialog queue and display
- `AudioManager.i` - Music/SFX playback
- `GlobalSettings.i` - UI colors and styling
- `GameLayers.i` - Physics layer masks

### Game State Flow
`GameController` owns the main game loop. In `FreeRoam` state, it delegates to `PlayerController.HandleUpdate()`. Dialog display pauses player input via `DialogManager.Instance.isShowing`.

### Mask System
Player can equip masks defined in `MaskType` enum (Default, Shaman, Tough, Ninja):
- **Ninja**: Can throw projectiles
- **Shaman**: Accesses astral plane (swaps normalPlane/astralPlane GameObjects)
- **ToughGuy**: Can push/pull `MovableObject` instances

### Character System
`Character.cs` is the base class for all characters (player and NPCs). Key components:
- Requires `Rigidbody2D`
- Uses `CharacterAnimator` for sprite animation
- Grid-based movement with tile snapping (`SetPositionAndSnapToTile`)
- Path collision checking via `GameLayers`

### Interaction System
Objects implement `IInteractable` interface. `PlayerController` detects interactables within range using `Physics2D.OverlapCircleAll` and highlights them via `HighlightSprite`.

### Dialog System
`DialogManager` uses a queue-based approach. Create `DialogData` with lines, choices, and callbacks, then call `QueueDialogToShow()` or yield `ShowDialogCoroutine()`.

### Scene Persistence
`EssentialObjects` prefab uses `DontDestroyOnLoad` to persist managers across scene transitions. Spawned by `EssentialObjectsSpawner` in each scene.

## Code Organization

```
Assets/Scripts/
├── GameController.cs          # Main game state machine
├── Character/                 # Player, guards, NPCs
├── Dialogues/                 # Dialog system
├── GamePlay/                  # GameLayers, GlobalSettings, interfaces
├── Items/                     # Collectible items and masks
├── UI/                        # Menu and inventory
├── Audio/                     # AudioManager
└── Util/Core/                 # EssentialObjects, Fader
```

## Key Dependencies

- **DOTween** (Demigiant): Used extensively for animations and transitions (`DOFade`, `DORotateQuaternion`)
- **Unity 2D Tilemap**: Grid-based level design with rule tiles

## Scenes

Build order: StartMenu → Level1 → Level2 → Level2_Maggie → Level3 → Level_ExitToJungle

## Input Handling

Interact keys (defined in `PlayerController.IsInteractInputKey`): Q, E, LeftControl, Space, Return, Mouse0
Menu keys (defined in `PlayerController.OpenMenuKeyDown`): Escape, KeypadEnter, Backspace
