# Unity Project -- System & Extension Documentation

## Document scope

This documentation is based on this project.
It describes the current implementation as it exists in
that upload: how the systems are connected, what each major script owns,
how the room editor/generator works, and practical procedures for adding
new content without breaking the existing architecture.

Where a system is only partially implemented, this document says so
instead of assuming a feature exists. The documentation is intentionally
written as a project-maintenance manual rather than a generic Unity
tutorial.

------------------------------------------------------------------------

# 1. High-level architecture

The project is organized around several mostly independent gameplay
systems:

``` text
                                    ┌──────────────────────┐
                                    │   InputSystem_Actions│
                                    └──────────┬───────────┘
                                               │
                    ┌──────────────────────────┼──────────────────────────┐
                    │                          │                          │
                    ▼                          ▼                          ▼
             PlayerMovement              PlayerCombat             PlayerInteraction
                    │                          │                          │
                    ▼                          ▼                          ▼
             PlayerStateMachine          Weapon/Collider          IInteractable
                                                                       │
                                                     ┌─────────────────┼──────────────┐
                                                     ▼                 ▼              ▼
                                                ItemPickup         QuestGiver     New interactables

Room Editor / Generation:

Room prefab → RoomConnector → RoomDatabase → RoomBuilder → LevelEditorSelection
                                                       │
                                                       ├── manual building
                                                       ├── Auto Build
                                                       └── cleanup / rebuild
                                                              │
                                                              ▼
                                                     AutoGenerateAndBake
                                                              │
                                                              ▼
                                                     NavMeshSurface

Data-driven content:

ItemData → ItemDatabase → Inventory → EquipmentManager / UI / SaveManager
QuestData → Quest → QuestManager → Quest UI
```

There are two broad kinds of objects in the project:

-   **Scene/runtime managers and components**: `LevelEditorSelection`,
    `QuestManager`, `Inventory`, `SaveManager`, `EnemySpawner`, etc.
-   **ScriptableObject data**: `RoomDatabase`, `ItemData`, `QuestData`.

The ScriptableObject pattern is important: content is stored as assets,
while runtime classes hold the current state of that content.

------------------------------------------------------------------------

# 2. File map -- all 66 scripts

## Room editor / level generation

  -----------------------------------------------------------------------
  Script                              Responsibility
  ----------------------------------- -----------------------------------
  `Room.cs`                           Defines a room's metadata,
                                      connectors, bounds, spawn points,
                                      selection state, and build history.

  `RoomConnector.cs`                  Defines a connection point, facing
                                      direction, anchor point, occupied
                                      state, connected room, and `+`
                                      indicator.

  `RoomBuilder.cs`                    Instantiates and rotates/snaps a
                                      room to a connector; also contains
                                      a reusable overlap test.

  `RoomDatabase.cs`                   ScriptableObject containing the
                                      list of room prefabs.

  `LevelEditorSelection.cs`           Main editor interaction, manual
                                      placement, Auto Build, required
                                      rooms, rebuilds, cleanup, and
                                      collision checking.

  `LevelEditorSelectionEditor.cs`     Custom Inspector for the level
                                      editor, including database-based
                                      Required Room selection.

  `LevelEditorManager.cs`             Singleton holding a room database
                                      and a selected connector. It is
                                      separate from the main
                                      `LevelEditorSelection` logic.

  `RoomButtonUI.cs`                   Room thumbnail/name button that
                                      tells `LevelEditorSelection` to
                                      build a room.

  `RoomSelectedUI.cs`                 Creates the room-selection UI from
                                      a `RoomDatabase`.

  `RoomOutline.cs`                    Runtime/editor-style room bounds
                                      outline using a `LineRenderer`.

  `RoomThumbnailGenerator.cs`         EditorWindow that renders every
                                      database room to PNG thumbnails.

  `LevelMapSaver.cs`                  Editor-only save of the generated
                                      room hierarchy as a prefab.

  `AutoGenerateAndBake.cs`            Starts Auto Build on scene start
                                      and bakes the runtime NavMesh after
                                      generation.

  `EditorCamera.cs`                   Camera movement/orbit/pan/zoom for
                                      the level editor.
  -----------------------------------------------------------------------

## Player

  -----------------------------------------------------------------------
  Script                              Responsibility
  ----------------------------------- -----------------------------------
  `Player.cs`                         Player-owned core values/state used
                                      by other player systems.

  `PlayerMovement.cs`                 CharacterController movement,
                                      sprint, jump, gravity, ground
                                      checks, and movement/state
                                      integration.

  `PlayerCombat.cs`                   Player attack input, cooldown,
                                      attack state, and weapon collider
                                      control.

  `PlayerStateMachine.cs`             Player animation/gameplay state
                                      mapping: idle, walking, sprint,
                                      jump, fall, etc.

  `PlayerInteraction.cs`              Finds nearby `IInteractable`
                                      objects, creates prompts, selects
                                      the closest, and calls
                                      `Interact()`.

  `PlayerUI.cs`                       Displays player-related UI values.

  `PlayerArrowUI.cs`                  Player/UI directional indicator.

  `CameraLook.cs`                     Runtime camera look/rotation.

  `Gravity.cs`                        Additional gravity/fall behavior
                                      component.

  `PlayerCollusionDetector.cs`        Player collision detection support.

  `Weapon.cs`                         Weapon hit detection and per-attack
                                      enemy hit tracking.

  `NormalAttackCollider.cs`           Normal attack collider helper.
  -----------------------------------------------------------------------

## Interaction / UI

  -----------------------------------------------------------------------
  Script                              Responsibility
  ----------------------------------- -----------------------------------
  `IInteractable.cs`                  Interface that defines
                                      `InteractionText` and `Interact()`.

  `InteractionUIManager.cs`           Singleton that creates/removes
                                      interaction prompt UI objects.

  `InteractionPromptUI.cs`            Displays interaction text and
                                      selected keybind.

  `ButtonBehaviour.cs`                UI hover/press scale and image
                                      behavior.

  `InputBindingUtility.cs`            Converts an Input System action
                                      into the preferred displayed
                                      binding.

  `MainMenuUIManager.cs`              Main menu UI behavior.

  `MenuManager.cs`                    General menu control.

  `SceneLoader.cs`                    Scene loading, Continue Game, and
                                      Quit.
  -----------------------------------------------------------------------

## Items / inventory / equipment

  -----------------------------------------------------------------------
  Script                              Responsibility
  ----------------------------------- -----------------------------------
  `ItemData.cs`                       ScriptableObject describing an
                                      item.

  `ItemDatabase.cs`                   Runtime lookup of `ItemData` by
                                      `itemID`.

  `ItemPickup.cs`                     `IInteractable` world pickup that
                                      adds an `ItemData` to inventory.

  `Inventory.cs`                      Runtime list of collected
                                      `ItemData` and UI/save refreshes.

  `InventorySlot.cs`                  Inventory slot UI.

  `InventoryUI.cs`                    Inventory display/refresh.

  `ItemPopupUI.cs`                    Item information popup.

  `ItemVisual.cs`                     Item visual presentation.

  `EquipmentManager.cs`               Equips items into compatible slots
                                      and returns previous equipment to
                                      inventory.

  `EquipmentSlot.cs`                  Validates and stores one equipped
                                      item.
  -----------------------------------------------------------------------

## Quests

  -----------------------------------------------------------------------
  Script                              Responsibility
  ----------------------------------- -----------------------------------
  `QuestData.cs`                      ScriptableObject definition of
                                      quest content and rewards.

  `QuestObjectiveData.cs`             Serialized objective definition.

  `QuestObjective.cs`                 Runtime objective progress.

  `Quest.cs`                          Runtime quest state and completion
                                      check.

  `QuestGiver.cs`                     `IInteractable` that accepts a
                                      quest.

  `QuestManager.cs`                   Active/completed quest lists,
                                      progress events, rewards, and UI
                                      refresh.

  `QuestUI.cs`                        Current tracked quest display.

  `QuestMenuUI.cs`                    Quest list/detail menu.

  `QuestButtonUI.cs`                  Quest list button.

  `QuestDetailsUI.cs`                 Quest detail display/tracking.
  -----------------------------------------------------------------------

## Enemies

  -----------------------------------------------------------------------
  Script                              Responsibility
  ----------------------------------- -----------------------------------
  `Enemy.cs`                          Enemy health, IDs, phases, death,
                                      and spawner reference.

  `EnemyAI.cs`                        Chase and patrol movement using
                                      NavMeshAgent.

  `EnemyCombat.cs`                    Enemy attack cooldown, attack
                                      types, and attack collider/meteor
                                      references.

  `EnemyStateMachine.cs`              Enemy animation/state handling.

  `EnemySpawner.cs`                   Spawn-once or infinite enemy
                                      spawning in a circle/box.

  `EnemyMiniMapDot.cs`                Pulsing minimap enemy indicator.

  `DebugScript.cs`                    Debug enemy-kill commands.
  -----------------------------------------------------------------------

## Saving / persistence

  -----------------------------------------------------------------------
  Script                              Responsibility
  ----------------------------------- -----------------------------------
  `SaveData.cs`                       Serializable save structure for
                                      player position, inventory, and
                                      equipment IDs.

  `SaveManager.cs`                    Save/load/autosave and save-file
                                      existence/deletion.
  -----------------------------------------------------------------------

## Other utility/content scripts

  Script               Responsibility
  -------------------- -------------------------------------------------
  `Meteor.cs`          Enemy attack/projectile behavior.
  `SkillSlotUI.cs`     Skill icon, cooldown, and displayed keybind UI.
  `EquipmentSlot.cs`   Equipment slot validation/storage.

------------------------------------------------------------------------

# 3. Room system -- the most important architecture

The room generator is built around four concepts:

1.  A **Room prefab** describes a room.
2.  A **RoomConnector** describes where another room can attach.
3.  A **RoomDatabase** tells the editor which room prefabs are
    available.
4.  **RoomBuilder** performs the actual instantiate/rotate/snap/connect
    operation.

`LevelEditorSelection` is the orchestrator. It decides *what* should be
built and *when*; `RoomBuilder` decides *how* the room is physically
attached.

------------------------------------------------------------------------

# 4. `Room.cs`

`Room` is the metadata and runtime identity of a room.

## Room types

The current enum is:

``` text
Start
Normal
DeadEnd
Boss
Treasure
Shop
Event
Stair
Secret
```

The type is used by Auto Build. The current generator specifically
treats `DeadEnd` specially: dead ends are never selected as normal
expansion rooms, do not count toward the room count, and are added after
the required-room/minimum-room checks.

The other types are currently available as categories, but Auto Build
does not have special placement rules for
Boss/Treasure/Shop/Event/Stair/Secret in the uploaded code. They behave
like ordinary non-dead-end rooms unless another system gives them
special meaning.

## Room sizes

``` text
Small
Medium
Large
Corridor
```

`Corridor` is special to Auto Build because corridor chains can be
exempt from the counted-room limit until `corridorChainLimit` is
exceeded.

## Orientation

`UpDirection` supports:

``` text
YUp
YDown
XUp
XDown
ZUp
ZDown
```

This is consumed by `RoomBuilder` when calculating the rotation needed
to connect a prefab.

## Important Room references

Each room can contain:

-   `RoomConnector` list
-   enemy spawn points
-   chest spawn points
-   player spawn points
-   a `BoxCollider` room bounds
-   visual root
-   thumbnail
-   selection indicator

The `RoomBounds` collider is particularly important for overlap testing.

## Build history

When `RoomBuilder.Build()` creates a room, it calls:

``` csharp
room.SetBuildHistory(
    previousRoom,
    connector,
    prefab);
```

This records:

-   `PreviousRoom`: the room this room was attached to.
-   `PreviousConnector`: the connector used on the previous room.
-   `SourcePrefab`: the original prefab asset used to create this
    instance.

`SourcePrefab` is also how Required Rooms are verified.

------------------------------------------------------------------------

# 5. Creating a new room prefab

This is the standard workflow.

## Step 1 -- create the physical room

Create the room geometry as a prefab.

The prefab should contain the visual meshes, floor/walls, colliders, and
any gameplay spawn points you want.

## Step 2 -- add `Room`

Add the `Room` component to the room root.

Configure:

``` text
Room Info
    Room Name
    Room Type
    Room Size

Orientation
    Up Direction

Room Bounds
    Room Bounds = BoxCollider
```

The Room `OnValidate()` scans child objects for `RoomConnector`
components and refreshes its connector list in the Unity Editor.

## Step 3 -- add connectors

Create one or more child objects with `RoomConnector`.

Each connector needs:

``` text
Direction
Anchor Point
Selection Indicator
```

The connector's `DirectionVector` is based on its configured
North/South/East/West direction.

The anchor point is the actual snap location.

## Step 4 -- make the connector visual

The current `RoomConnector` caches renderers from its children. The `+`
visual is hidden automatically when `occupied == true` by
`UpdateVisibility()`.

The separate `selectionIndicator` is controlled by:

``` csharp
connector.SetSelected(true);
connector.SetSelected(false);
```

So the `+`/selection indicator needs to be assigned correctly on the
prefab.

## Step 5 -- assign Room Bounds

This is critical for collision detection.

The `RoomBounds` should be a BoxCollider that represents the room's
usable footprint.

For normal Auto Build rooms, the generator compares the new room's
bounds with existing generated room bounds using
`Physics.ComputePenetration`.

Do not make the bounds so large that every legitimate connector
connection becomes a collision.

## Step 6 -- add to the Room Database

Open your `Room Database` asset and add the prefab to its `rooms` list.

The room will then appear in the room-selection UI and can be considered
by Auto Build.

## Step 7 -- generate thumbnail

Use:

``` text
Tools → Room Thumbnail Generator
```

Assign the Room Database and click `Generate All Thumbnails`.

------------------------------------------------------------------------

# 6. `RoomConnector.cs` -- connector lifecycle

A connector has four important pieces of state:

``` text
Facing
Occupied
AnchorPoint
ConnectedRoom
```

## Free connector

``` text
Occupied = false
```

It is eligible for building and its renderer visibility system allows
the connector visual to appear.

## Used connector

`RoomBuilder.Build()` sets:

``` csharp
connector.Occupied = true;
connector.ConnectedRoom = room;
```

The destination connector on the newly built room is also marked
occupied and connected back to the previous room.

## Important distinction: renderer vs selection indicator

The script has two related but different mechanisms:

1.  `UpdateVisibility()` hides/shows all child renderers according to
    `occupied`.
2.  `SetSelected()` activates/deactivates the assigned
    `selectionIndicator` GameObject.

This is why changing `Occupied` and changing the selected `+` indicator
are not exactly the same operation.

------------------------------------------------------------------------

# 7. `RoomBuilder.cs` -- how room placement physically works

`RoomBuilder.Build(connector, prefab)` does this:

``` text
Validate connector/prefab
        ↓
Check source connector isn't occupied
        ↓
Instantiate prefab
        ↓
Find first available connector on new room
        ↓
Calculate target direction
        ↓
Calculate source connector direction
        ↓
Calculate rotation
        ↓
Rotate room
        ↓
Snap target connector anchor to source connector anchor
        ↓
Mark both connectors occupied
        ↓
Set build history
        ↓
Return Room instance
```

The important mathematical idea is that the new room's connector
direction is rotated to face the opposite direction of the connector
being used.

The room's `UpDirection` is used to build the source rotation.

## Do not duplicate this logic

If you create another room-generation feature, use:

``` csharp
RoomBuilder.Build(connector, prefab);
```

instead of writing another instantiate/rotation/snap implementation.

That keeps all connector alignment behavior in one place.

------------------------------------------------------------------------

# 8. Room collision checking

There are currently two collision concepts in the uploaded project.

## `RoomBuilder.IsOverlapping()`

This uses `Physics.OverlapBox()` around the new room's bounds and
ignores:

-   colliders belonging to the new room
-   the room being connected to

## `LevelEditorSelection.IsRoomOverlappingExistingRooms()`

Auto Build currently uses its own bounds-based check with
`Physics.ComputePenetration()`.

It loops over the rooms tracked in `autoBuiltRooms` and compares
`RoomBounds`.

This is the active collision path for Auto Build in the uploaded
`LevelEditorSelection.cs`.

## Dead ends

The current `TryBuildDeadEnd()` deliberately does **not** perform
collision checking. It simply attaches a dead-end prefab to the
available connector and records it in `autoBuiltRooms`.

That is an intentional rule in the current system.

------------------------------------------------------------------------

# 9. `RoomDatabase.cs`

`RoomDatabase` is a ScriptableObject:

``` csharp
[CreateAssetMenu(
    fileName = "Room Database",
    menuName = "Level Editor/Room Database")]
```

Create one with:

``` text
Project → Right Click
→ Create
→ Level Editor
→ Room Database
```

Then populate its `rooms` list with room prefabs.

## What the database is used for

The database is used by:

-   `LevelEditorSelection` Auto Build candidate selection.
-   `RoomSelectedUI` to create the room buttons.
-   `LevelEditorSelectionEditor` to create the Required Rooms menu.
-   `RoomThumbnailGenerator` to generate thumbnails for all rooms.

## Adding another database

If you want a second room collection, the cleanest current approach is
simply to create another `RoomDatabase` asset.

For example:

``` text
DungeonDatabase
    SmallRoom
    LargeRoom
    BossRoom

SewerDatabase
    SewerStraight
    SewerCorner
    SewerLarge
```

However, the current `LevelEditorSelection` accepts only one
`RoomDatabase` reference at a time. To switch databases, change that
Inspector reference, or extend the editor/runtime logic if you want
multiple databases active simultaneously.

------------------------------------------------------------------------

# 10. Room selection UI

`RoomSelectedUI` reads the database and creates one button for each
room.

Each button uses `RoomButtonUI`.

The general flow is:

``` text
RoomDatabase
      ↓
RoomSelectedUI.GenerateRoomButtons()
      ↓
RoomButtonUI.Setup(room, editor)
      ↓
Player clicks button
      ↓
RoomButtonUI.OnClick()
      ↓
LevelEditorSelection.BuildSelectedRoom(room)
```

This means a new room normally requires **no new UI script**. Add the
prefab to the database and generate its thumbnail; the room-selection UI
can use it automatically.

------------------------------------------------------------------------

# 11. How to add a new room type

If you want something such as:

``` text
Puzzle
```

add it to `Room.RoomType`:

``` csharp
public enum RoomType
{
    Start,
    Normal,
    DeadEnd,
    Boss,
    Treasure,
    Shop,
    Event,
    Stair,
    Secret,
    Puzzle
}
```

That alone gives the Inspector a new category, but it does **not**
automatically change Auto Build behavior.

If Puzzle rooms need special generation rules, add the rule inside
`LevelEditorSelection.TryBuildRoom()` or create a dedicated selection
helper.

For example:

``` csharp
if (prefab.Type == Room.RoomType.Puzzle && !allowPuzzleRooms)
    continue;
```

If Puzzle rooms should be required, the existing Required Rooms system
already supports them because it compares the exact prefab reference
through `SourcePrefab`.

------------------------------------------------------------------------

# 12. Room thumbnails -- complete workflow

`RoomThumbnailGenerator` is Editor-only because it uses `UnityEditor`
and `EditorWindow`.

## Requirements

Create a Unity layer named exactly:

``` text
RoomPreview
```

The generator refuses to run if this layer does not exist.

## Process

For each room in the selected database:

``` text
Instantiate temporary prefab
        ↓
Put room and children on RoomPreview layer
        ↓
Find all renderers
        ↓
Calculate combined bounds
        ↓
Position temporary camera
        ↓
Render into RenderTexture
        ↓
Read pixels into Texture2D
        ↓
Encode PNG
        ↓
Save to Assets/Room Thumbnails
        ↓
Destroy temporary objects
```

Default output folder:

``` text
Assets/Room Thumbnails
```

Default image size:

``` text
512 × 512
```

## If a thumbnail is missing

Check:

1.  Room is actually in the selected database.
2.  Room has at least one `Renderer`.
3.  `RoomPreview` layer exists.
4.  Output folder is valid.
5.  The room's renderer hierarchy is not disabled unexpectedly.

## After creating a new room

Recommended order:

``` text
Create prefab
→ Configure Room
→ Configure connectors
→ Add to database
→ Generate thumbnails
→ Test manual placement
→ Test Auto Build
→ Save map
```

------------------------------------------------------------------------

# 13. Saving a generated level

`LevelMapSaver` is Editor-only for the actual save operation.

It takes:

``` text
Generated Rooms Root
```

and saves the entire hierarchy as a prefab using:

``` csharp
PrefabUtility.SaveAsPrefabAsset()
```

Default folder:

``` text
Assets/Levels
```

## Save workflow

``` text
Build map
    ↓
Save button
    ↓
Choose prefab path
    ↓
Generated Rooms hierarchy saved as prefab
```

The saver also records prefab instance property modifications before
saving.

## Important distinction

This saves the **generated room hierarchy**. It is not the same thing as
saving the random-generation settings or a procedural seed.

If you later want deterministic map recreation, a seed system would need
to be added separately.

------------------------------------------------------------------------

# 14. Auto Build -- exact current workflow

The current `LevelEditorSelection.AutoBuild()` validates:

-   Auto Build is not already running.
-   Room Database exists.
-   Database contains rooms.
-   Starting Room exists.
-   Generated Rooms Root exists.

It then starts `AutoBuildRoutine()`.

## Attempt lifecycle

``` text
Start attempt
    ↓
Clear previous Auto Build rooms
    ↓
Instantiate Starting Room
    ↓
Reset starting connectors
    ↓
Check that starting room has connectors
    ↓
GenerateMap()
    ↓
Check minimum room count
    ↓
Check required rooms
    ↓
Add Dead Ends
    ↓
Refresh connector indicators
    ↓
SUCCESS
```

If minimum room count or required-room validation fails:

``` text
Clear Auto Build rooms
        ↓
New attempt
```

This repeats until either:

-   requirements succeed, or
-   `maxRebuildAttempts` is reached, or
-   Infinite Rebuild is enabled and a successful map eventually occurs.

------------------------------------------------------------------------

# 15. Auto Build room counting

The current count rules are:

### Dead End

Never counted.

### Non-corridor room

Counts as one room.

### Corridor

If `countLongCorridorChains` is false, corridors do not count.

If it is true:

``` text
Corridor chain limit = 3

Corridor 1 → free
Corridor 2 → free
Corridor 3 → free
Corridor 4 → counts
Corridor 5 → counts
...
```

The chain is calculated by walking backward through `PreviousRoom` while
the previous rooms are corridors.

------------------------------------------------------------------------

# 16. Maximum Rooms

`maximumRooms` is checked by `GenerateMap()` before another expansion
iteration.

When the counted-room number reaches the maximum, normal expansion
stops.

Dead Ends are separate and are not counted.

This means:

``` text
Maximum Rooms = 20

Counted rooms = 20
Dead Ends = 8

Total physical rooms = 28
```

That is valid under the current design because Dead Ends are not
counted.

------------------------------------------------------------------------

# 17. Minimum Rooms

`minimumRooms` is checked after `GenerateMap()` finishes.

If:

``` text
countedRooms < minimumRooms
```

the attempt is discarded and rebuilt.

The minimum therefore controls whether an Auto Build attempt is
accepted; it does not force the generator to magically create a room if
no valid placement exists.

------------------------------------------------------------------------

# 18. Required Rooms

Required Rooms are stored as prefab references:

``` csharp
private List<Room> requiredRooms;
```

The custom Inspector lets you choose a room from the Room Database
instead of dragging the prefab manually.

At runtime the generated room is checked using:

``` csharp
generated.SourcePrefab == required
```

So the generated instance does not have to be the same GameObject
reference as the prefab. Its `SourcePrefab` must point to the required
prefab.

## Adding a Required Room

Inspector:

``` text
Required Rooms
    + Add Required Room
```

Choose a room from the database.

No runtime code is required.

------------------------------------------------------------------------

# 19. Adding special Auto Build rules

If you want a rule such as:

> Boss room should only appear once.

A good place is a helper called from `TryBuildRoom()`.

Conceptually:

``` csharp
if (prefab.Type == Room.RoomType.Boss && HasBossRoomAlready())
    continue;
```

Then implement:

``` csharp
private bool HasBossRoomAlready()
{
    foreach (Room room in autoBuiltRooms)
    {
        if (room != null &&
            room.Type == Room.RoomType.Boss)
            return true;
    }

    return false;
}
```

For a more advanced system, use room tags/rules rather than accumulating
many `if` statements in `TryBuildRoom()`.

------------------------------------------------------------------------

# 20. Runtime generation and NavMesh

`AutoGenerateAndBake` connects runtime generation to navigation.

Its workflow is:

``` text
Start()
 ↓
AutoBuild()
 ↓
Wait while IsAutoBuildRunning
 ↓
Wait configured number of frames
 ↓
Physics.SyncTransforms()
 ↓
NavMeshSurface.BuildNavMesh()
```

## Setup

On a persistent Map object:

``` text
Map
├── NavMeshSurface
└── AutoGenerateAndBake
```

Assign:

``` text
Level Editor → LevelEditorSelection
Nav Mesh Surface → NavMeshSurface
```

The current script expects `LevelEditorSelection` to expose:

``` csharp
public bool IsAutoBuildRunning => autoBuildRunning;
```

## Important

This is a **runtime NavMesh build**. It creates the navigation mesh for
the current runtime session. It is not the same as permanently saving an
editor-baked NavMesh asset.

------------------------------------------------------------------------

# 21. How to add a new interactable

This is one of the cleanest extension points in the project.

The interface is:

``` csharp
public interface IInteractable
{
    string InteractionText { get; }

    void Interact();
}
```

Any MonoBehaviour can become interactable by implementing it.

## Example: door

``` csharp
using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string interactionText = "Open Door";

    private bool opened;

    public string InteractionText =>
        interactionText;

    public void Interact()
    {
        if (opened)
            return;

        opened = true;

        Debug.Log("Door opened.");

        // Add your door animation here.
    }
}
```

## Scene setup

The player interaction system detects the interface through a trigger:

``` text
Player
    PlayerInteraction
        trigger collider

Door
    DoorInteractable
    collider
```

When the player enters the trigger, `PlayerInteraction` creates an
interaction prompt through `InteractionUIManager`.

When the player presses the Interact input:

``` text
PlayerInteraction
    ↓
currentInteractable.Interact()
```

## You do NOT need to edit PlayerInteraction

This is the key advantage of the interface design.

For most new interactions, create a new class implementing
`IInteractable`.

------------------------------------------------------------------------

# 22. Interaction prompt system

`InteractionUIManager` is a singleton:

``` csharp
InteractionUIManager.Instance
```

It creates prompts using a prefab.

`InteractionPromptUI.Setup()` fills:

-   interaction text
-   displayed input binding

The selected prompt shows the keybind root; non-selected prompts hide
it.

The closest interactable is selected by distance in
`PlayerInteraction.UpdateCurrentInteractable()`.

------------------------------------------------------------------------

# 23. Adding a new item

Items are data assets, not MonoBehaviours.

Create:

``` text
Project → Right Click
→ Create
→ Inventory
→ Item
```

Fill:

``` text
Item ID
Item Name
Icon
Item Type
Description
```

Current item types:

``` text
Weapon
Helmet
Chest
Leggings
Boots
Ring
Additional
Consumable
```

## Item ID

The `itemID` is important because `ItemDatabase.GetItem(string id)` uses
it for lookup and the save system stores item IDs.

Use stable unique IDs such as:

``` text
weapon_iron_sword
armor_leather_helmet
consumable_health_potion
```

Avoid changing an item's ID after save files may already contain it.

------------------------------------------------------------------------

# 24. Adding an item pickup

Create a GameObject in the scene and add:

``` text
ItemPickup
Collider
```

Assign its `ItemData`.

`ItemPickup` implements `IInteractable`, so the normal interaction
system handles it.

When interacted with:

``` text
ItemPickup.Interact()
        ↓
Inventory.AddItem(item)
        ↓
InventoryUI.Refresh()
        ↓
SaveManager.AutoSave()
        ↓
Pickup object destroyed
```

No change to `PlayerInteraction` is necessary.

------------------------------------------------------------------------

# 25. Adding a new item type

Add the enum value in `ItemData.cs`:

``` csharp
public enum ItemType
{
    Weapon,
    Helmet,
    Chest,
    Leggings,
    Boots,
    Ring,
    Additional,
    Consumable,
    QuestItem
}
```

But adding the enum value alone is not enough if the new type needs
special behavior.

For example, a new equipment type also needs an `EquipmentSlot`
configured to accept that type.

------------------------------------------------------------------------

# 26. Item database

`ItemDatabase` is a runtime MonoBehaviour singleton:

``` csharp
ItemDatabase.Instance
```

It contains an array of `ItemData` assets and exposes:

``` csharp
GetItem(string id)
```

Use the database when loading saved item IDs or when a system needs to
find an item from an ID.

### Important distinction

`RoomDatabase` is a ScriptableObject.

`ItemDatabase` is currently a MonoBehaviour.

Do not assume all databases in this project follow the same
architecture.

------------------------------------------------------------------------

# 27. Equipment system

`EquipmentManager` gets an `Inventory` from the same GameObject in
`Start()`.

To equip an item:

``` text
EquipmentManager.Equip(item)
        ↓
loop through EquipmentSlot[]
        ↓
slot.CanEquip(item)
        ↓
slot.Equip(item)
        ↓
previous equipment goes back to inventory
        ↓
new item removed from inventory
```

Each `EquipmentSlot` has an accepted `ItemType`.

To add a new equipment slot:

1.  Add a new UI slot GameObject.
2.  Add `EquipmentSlot`.
3.  Set `Accepted Type`.
4.  Set its icon Image.
5.  Add the slot to `EquipmentManager.slots`.

------------------------------------------------------------------------

# 28. Adding a new quest

Create:

``` text
Project → Right Click
→ Create
→ Quest
→ Quest Data
```

Set:

``` text
Quest ID
Quest Name
Description
Objectives
Gold Reward
XP Reward
Item Rewards
```

The current objective types are defined in `QuestObjectiveData.cs`.

The runtime model is:

``` text
QuestData
   ↓
Quest
   ↓
QuestObjective
```

`QuestData` is static content; `Quest` stores runtime state.

------------------------------------------------------------------------

# 29. Quest objective progression

`QuestManager` exposes event-like methods:

``` csharp
EnemyKilled(string enemyID)
ItemCollected(string itemID)
NPCTalked(string npcID)
AreaReached(string areaID)
ObjectInteracted(string objectID)
```

All eventually call:

``` csharp
UpdateObjectives(objectiveType, targetID)
```

An objective progresses when both match:

``` text
ObjectiveType
Target ID
```

For example:

``` text
Objective Type = Kill
Target ID = goblin
Required Amount = 5
```

A kill system should call:

``` csharp
QuestManager.Instance.EnemyKilled("goblin");
```

five times.

------------------------------------------------------------------------

# 30. Adding a new quest objective type

Add a new enum value in `QuestObjectiveData.cs`.

For example:

``` csharp
public enum ObjectiveType
{
    Kill,
    Collect,
    Talk,
    ReachArea,
    Interact,
    OpenChest
}
```

Then add a forwarding method in `QuestManager`:

``` csharp
public void ChestOpened(string chestID)
{
    UpdateObjectives(
        ObjectiveType.OpenChest,
        chestID);
}
```

The system's central matching logic already exists in
`UpdateObjectives()`.

------------------------------------------------------------------------

# 31. Adding a quest giver

Add `QuestGiver` to an NPC/object.

Assign its `QuestData` and interaction text.

It automatically implements `IInteractable` and checks:

1.  Quest is not already active.
2.  Quest is not already completed.
3.  Otherwise accept it through `QuestManager`.

------------------------------------------------------------------------

# 32. Enemy system

Enemy architecture is split into:

``` text
Enemy
├── health / phases / death
├── EnemyAI
│     └── NavMeshAgent movement
├── EnemyCombat
└── EnemyStateMachine
```

`EnemySpawner` creates enemies and tracks alive count.

## Enemy ID

`Enemy.enemyID` is important for quests because
`QuestManager.EnemyKilled(enemyID)` can use the same string as the quest
target ID.

Keep IDs stable and unique.

Example:

``` text
Enemy ID = goblin_basic
Quest target ID = goblin_basic
```

------------------------------------------------------------------------

# 33. Enemy phases

`Enemy` supports phase health through:

``` csharp
int[] phaseHealth
```

It tracks:

``` text
CurrentPhase
CurrentHealth
HealthPercent
IsDead
```

Damage can trigger `NextPhase()` and `EnterPhase()` depending on
configured phase health.

------------------------------------------------------------------------

# 34. Enemy spawning

`EnemySpawner` supports:

``` text
SpawnMode
    SpawnOnce
    Infinite

SpawnShape
    Circle
    Box
```

For infinite spawning, it tracks `maxAlive` and uses a respawn delay.

A spawned enemy is given the spawner reference so that enemy death can
tell the spawner that one slot has become available.

------------------------------------------------------------------------

# 35. Player movement and state system

The player movement system uses:

-   `CharacterController`
-   `InputSystem_Actions`
-   `PlayerStateMachine`
-   `PlayerCombat`

Movement handles:

-   move input
-   speed calculation
-   sprint
-   jump
-   gravity
-   ground checking
-   falling
-   movement permission during combat

`PlayerStateMachine` maps states to handlers and animation parameters.

When adding a new player state, update the enum and the state-action
mapping/animation handling in `PlayerStateMachine`.

------------------------------------------------------------------------

# 36. Adding a new player ability

A good pattern is:

``` text
Input Action
    ↓
Player ability component
    ↓
PlayerStateMachine state (if the ability changes state)
    ↓
Animator
    ↓
Gameplay effect
```

For example, a dodge could use:

``` text
Dodge input
 ↓
PlayerDodge
 ↓
Dodge state
 ↓
Animator trigger
 ↓
Movement lock / movement burst
```

Avoid putting every ability directly into `PlayerMovement.cs`; that file
already owns movement/gravity responsibilities.

------------------------------------------------------------------------

# 37. Player combat

`PlayerCombat` listens for the attack input and manages:

-   attack cooldown
-   attack state
-   Animator trigger
-   weapon collider

It exposes methods for enabling/disabling the weapon collider.

`Weapon` uses a `HashSet<Enemy>` so an enemy is not repeatedly hit by
the same attack collider activation.

The intended attack flow is:

``` text
Attack input
 ↓
PlayerCombat
 ↓
Animator attack
 ↓
Animation event / timing
 ↓
Enable weapon collider
 ↓
Weapon.OnTriggerEnter
 ↓
Enemy.TakeDamage
 ↓
Disable weapon collider
```

------------------------------------------------------------------------

# 38. Save system

`SaveData` currently contains:

-   player X/Y/Z
-   inventory item IDs
-   equipped item IDs

`SaveManager` owns saving/loading and autosaving.

## Why IDs are saved instead of ScriptableObject references

Save data stores strings because asset references should not be
serialized as raw runtime object references in a simple JSON-style save
format.

On load, the system can resolve IDs through `ItemDatabase`.

## Adding a new saved value

Example: player gold.

### Step 1 -- add it to `SaveData`

``` csharp
public int gold;
```

### Step 2 -- write it during Save

Find the place where `SaveData` is constructed and assign:

``` csharp
data.gold = playerGold;
```

### Step 3 -- restore it during Load

Read:

``` csharp
playerGold = data.gold;
```

### Step 4 -- test old saves

Old save files may not contain the new value. Choose a sensible default
such as zero.

------------------------------------------------------------------------

# 39. Scene loading

`SceneLoader` provides:

``` text
LoadScene(string sceneName)
ContinueGame()
QuitGame()
```

`SaveManager` holds a reference to `SceneLoader` because loading a saved
game may require scene loading before restoring player state.

------------------------------------------------------------------------

# 40. Editor camera

`EditorCamera` is separate from the gameplay camera.

It uses the Input System for:

-   keyboard movement
-   orbit
-   pan
-   zoom
-   sprint

It uses raycasting to find a world point for orbit/pan operations.

If you add a new editor navigation mode, extend `EditorCamera` rather
than `LevelEditorSelection`.

------------------------------------------------------------------------

# 41. UI button system

`ButtonBehaviour` is a generic visual enhancement component for Unity UI
buttons.

It stores:

-   original scale
-   hover scale
-   pressed scale
-   original image
-   hover image

It responds to pointer events.

This system does not control the actual gameplay action of the button;
it handles presentation.

------------------------------------------------------------------------

# 42. Input System pattern used by the project

Several scripts create their own generated:

``` csharp
InputSystem_Actions
```

and enable/disable it in `OnEnable()`/`OnDisable()`.

When adding a new input-driven system:

1.  Add the action to the Input Actions asset.
2.  Regenerate the C# wrapper if required by your Input System setup.
3.  Subscribe in `OnEnable()`.
4.  Unsubscribe in `OnDisable()`.
5.  Read/process the input in the component that owns the feature.

Do not assume another component's input object will exist unless the
feature already exposes it.

------------------------------------------------------------------------

# 43. Recommended method for adding any new feature

Before writing code, decide which category the feature belongs to.

## A. Is it content/data?

Use a ScriptableObject.

Examples:

``` text
ItemData
QuestData
RoomDatabase
```

## B. Is it behavior attached to a GameObject?

Use a MonoBehaviour.

Examples:

``` text
ItemPickup
QuestGiver
EnemySpawner
DoorInteractable
```

## C. Is it a cross-object contract?

Use an interface.

The existing example is:

``` text
IInteractable
```

## D. Is it editor-only functionality?

Use `#if UNITY_EDITOR` and/or an `EditorWindow` / `CustomEditor`.

Examples:

``` text
RoomThumbnailGenerator
LevelEditorSelectionEditor
LevelMapSaver
```

------------------------------------------------------------------------

# 44. Adding a completely new database

There are two existing patterns.

## Pattern A -- ScriptableObject database

This is what `RoomDatabase` uses.

Use this for content that should exist as an asset and be reusable in
many scenes.

Example:

``` csharp
[CreateAssetMenu(
    fileName = "Enemy Database",
    menuName = "Game/Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    public List<Enemy> enemies = new();
}
```

Then create the asset in the Project window and assign it to systems
that need it.

## Pattern B -- scene/runtime database

`ItemDatabase` currently uses a MonoBehaviour singleton and an array of
`ItemData`.

Use this style if the database is scene-owned and you want a central
runtime lookup service.

For a new system, prefer the ScriptableObject style when the data is
content rather than runtime state.

------------------------------------------------------------------------

# 45. Adding a new database to an editor UI

If you want a database-driven selection menu like Required Rooms:

``` text
Database asset
    ↓
Custom Inspector
    ↓
GenericMenu
    ↓
Select asset
    ↓
SerializedProperty.objectReferenceValue
```

`LevelEditorSelectionEditor` is the existing example.

The important part is that the Inspector stores the actual asset
reference, not the display name.

------------------------------------------------------------------------

# 46. Adding a new thumbnail generator

Copy the architecture of `RoomThumbnailGenerator`:

``` text
EditorWindow
 ↓
Select database
 ↓
Create temporary camera
 ↓
Create temporary instance
 ↓
Isolate rendering with a layer
 ↓
Calculate renderer bounds
 ↓
RenderTexture
 ↓
Texture2D.ReadPixels
 ↓
Encode PNG
 ↓
AssetDatabase.Refresh
```

The current generator is intentionally isolated from the runtime room
editor. It does not alter the actual room prefab; it creates a temporary
instance, renders it, and destroys it.

------------------------------------------------------------------------

# 47. Adding a new runtime generated room decoration system

If you want generated rooms to receive decorations, use the room's spawn
points.

For example:

``` text
Room
 ├── EnemySpawnPoints
 ├── ChestSpawnPoints
 └── PlayerSpawnPoints
```

After Auto Build completes, iterate through generated rooms and spawn
the required content at those points.

Do not modify `RoomBuilder` just to spawn enemies/chests unless the
decoration must happen during the actual placement operation.

A separate `RoomRuntimeInitializer` would be cleaner:

``` text
Auto Build
 ↓
Room generation complete
 ↓
RoomRuntimeInitializer
 ↓
spawn enemies / chests / loot
 ↓
NavMesh bake
```

------------------------------------------------------------------------

# 48. Recommended runtime startup order

With the current systems, the cleanest order is:

``` text
Scene loads
   ↓
Managers Awake
   ↓
Room editor/generator available
   ↓
AutoGenerateAndBake.Start()
   ↓
LevelEditorSelection.AutoBuild()
   ↓
Generated rooms
   ↓
Required/minimum validation
   ↓
Dead Ends
   ↓
NavMeshSurface.BuildNavMesh()
   ↓
Enemy AI / NavMeshAgent gameplay
```

If enemies are spawned before the NavMesh exists, their `NavMeshAgent`
may not be able to navigate. Therefore, when adding runtime enemy
spawning to generated rooms, make sure it happens after the NavMesh is
ready, or explicitly initialize the agents after baking.

------------------------------------------------------------------------

# 49. Troubleshooting room generation

## Auto Build does nothing

Check:

``` text
LevelEditorSelection
    Room Database assigned
    Starting Room assigned
    Generated Rooms Root assigned
```

Then check the starting prefab has at least one unoccupied
`RoomConnector`.

The current generator explicitly rejects a starting room with zero
available connectors.

## Starting room appears but no expansion happens

Check:

-   connector exists in the Room's connector list
-   connector is not occupied
-   `AnchorPoint` is assigned
-   connector direction is valid
-   database contains rooms with at least one free connector
-   candidate rooms are not rejected by collision bounds

## Rooms overlap

Check each room's `RoomBounds` BoxCollider.

The collision system is only as good as those bounds.

## Every room is rejected

Your bounds are probably too large, rotated incorrectly, or touching
another room beyond the intended connector contact.

## Dead Ends overlap

That is expected from the current requested behavior: **dead ends
intentionally skip collision checking**.

## Dead Ends count toward Maximum Rooms

They should not under the current `GetCountedRoomCount()`
implementation. Verify the room's `RoomType` is actually `DeadEnd`.

------------------------------------------------------------------------

# 50. Troubleshooting thumbnails

## Thumbnail generator says RoomPreview layer does not exist

Create a layer named:

``` text
RoomPreview
```

exactly.

## Thumbnail is blank

Check that the room has active renderers.

## Thumbnail is too small

Increase `Image Size` or adjust the room's renderer bounds/camera
framing logic.

## Thumbnail shows unrelated objects

The generator puts the temporary room hierarchy onto the `RoomPreview`
layer and the temporary camera renders only that layer. Check that the
layer is actually applied recursively.

------------------------------------------------------------------------

# 51. Troubleshooting interaction

## No prompt appears

Check:

-   `PlayerInteraction` is on the player.
-   player trigger collider is configured correctly.
-   target object implements `IInteractable`.
-   `InteractionUIManager.Instance` exists.
-   `promptPrefab` is assigned.
-   `promptContainer` is assigned.

## Prompt appears but pressing Interact does nothing

Check the Input System action:

``` text
Player → Interact
```

and make sure the generated `InputSystem_Actions` wrapper matches the
current Input Actions asset.

## Wrong interactable is selected

The current implementation chooses the interactable with the smallest
world-space distance from the player's transform.

It does not perform a facing-direction/line-of-sight priority test.

------------------------------------------------------------------------

# 52. Troubleshooting item saving

If an item loads as missing, check its `itemID`.

The saved data uses item IDs, and `ItemDatabase.GetItem(id)` searches
the configured database array.

Common problem:

``` text
Old save:
weapon_old_sword

ItemData now:
weapon_iron_sword
```

The old save will no longer find the renamed ID.

Treat IDs as stable identifiers.

------------------------------------------------------------------------

# 53. Troubleshooting quests

If a quest objective does not progress, compare exactly:

``` text
QuestObjectiveData.objectiveType
QuestObjectiveData.targetID
```

with the method being called by the gameplay system.

Example:

``` csharp
QuestManager.Instance.EnemyKilled(enemy.EnemyID);
```

must match:

``` text
Objective Type = Kill
Target ID = enemy.EnemyID
```

String IDs are case-sensitive in the current comparisons.

------------------------------------------------------------------------

# 54. Troubleshooting NavMesh

If generated enemies do not move:

1.  Confirm Auto Build actually finished.
2.  Confirm `AutoGenerateAndBake` has a `LevelEditorSelection`
    reference.
3.  Confirm it has a `NavMeshSurface` reference.
4.  Confirm `IsAutoBuildRunning` becomes false.
5.  Confirm `BuildNavMesh()` is called.
6.  Confirm NavMesh Surface's `Collect Objects` includes the generated
    room geometry.
7.  Confirm the generated floor geometry is included by the chosen
    `Use Geometry` mode.
8.  Confirm the enemy is spawned after the NavMesh is ready.

------------------------------------------------------------------------

# 55. Current architectural caveats

These are important when extending the project.

## `LevelEditorManager` vs `LevelEditorSelection`

There are two level-editor-related managers.

`LevelEditorManager` is a singleton with:

``` text
RoomDatabase
SelectedConnector
```

`LevelEditorSelection` owns the actual selection and Auto Build workflow
used by the current scripts.

Do not accidentally create a third source of truth for selected
connectors or databases.

If the project continues growing, consolidating these two
responsibilities would reduce confusion.

## RoomDatabase vs ItemDatabase

They use different architectures:

``` text
RoomDatabase = ScriptableObject
ItemDatabase = MonoBehaviour singleton
```

Follow the existing pattern of the subsystem you are modifying, or
deliberately standardize them later.

## Runtime generation vs editor saving

`AutoGenerateAndBake` generates at runtime.

`LevelMapSaver` saves an editor hierarchy as a prefab.

These are separate workflows.

## Dead End collision behavior

Dead Ends currently skip collision checks by design.

If this is changed later, modify only the dead-end placement path rather
than changing the normal room collision logic accidentally.

------------------------------------------------------------------------

# 56. Recommended project workflow for new content

## New room

``` text
1. Model/build room
2. Add Room
3. Add RoomConnector(s)
4. Set connector directions
5. Set AnchorPoint(s)
6. Add selection indicator
7. Configure RoomBounds
8. Set RoomType
9. Set RoomSize
10. Add spawn points
11. Add prefab to RoomDatabase
12. Generate thumbnail
13. Manual-build test
14. Collision test
15. Auto-Build test
16. Save test map
```

## New interactable

``` text
1. Create MonoBehaviour
2. Implement IInteractable
3. Implement InteractionText
4. Implement Interact()
5. Add collider/trigger setup
6. Put it in scene/prefab
7. Test prompt
8. Test interaction
```

## New item

``` text
1. Create ItemData
2. Assign stable itemID
3. Fill name/icon/type/description
4. Add ItemData to ItemDatabase
5. Create ItemPickup if it is a world pickup
6. Test inventory
7. Test save/load
```

## New quest

``` text
1. Create QuestData
2. Set stable questID
3. Add objectives
4. Set target IDs
5. Set rewards
6. Assign QuestData to QuestGiver
7. Trigger matching QuestManager progress method
8. Test completion
```

## New enemy

``` text
1. Create enemy prefab
2. Add Enemy
3. Set Enemy ID
4. Configure health/phases
5. Add EnemyAI
6. Add NavMeshAgent
7. Add EnemyCombat
8. Add EnemyStateMachine
9. Configure attack references
10. Put prefab into EnemySpawner
11. Test after NavMesh generation
```

------------------------------------------------------------------------

# 57. Safe extension rule

When adding a feature, prefer this order:

``` text
Existing interface/API
        ↓
Existing data asset
        ↓
New component
        ↓
Existing manager notification
        ↓
UI only if required
```

Avoid immediately editing large central scripts.

For example, a new interactable should implement `IInteractable`; it
should not require changes to `PlayerInteraction`.

A new item should use `ItemData`; it should not require hard-coded item
names in Inventory.

A new required room should use the existing `requiredRooms` list; it
should not require hard-coded prefab references.

------------------------------------------------------------------------

# 58. Practical naming rules

Use stable IDs for data-driven content.

Recommended:

``` text
room_boss_01
room_corridor_long
item_iron_sword
item_health_potion
enemy_goblin_basic
enemy_goblin_elite
quest_find_sword
npc_blacksmith
```

Do not use display names as IDs.

Display name can change:

``` text
Iron Sword → Rusted Iron Sword
```

ID should remain:

``` text
item_iron_sword
```

------------------------------------------------------------------------

# 59. Quick reference -- which script should I edit?

  -----------------------------------------------------------------------
  Goal                                Main file to edit
  ----------------------------------- -----------------------------------
  Add a room property                 `Room.cs`

  Add a room category                 `Room.cs`

  Change connector behavior           `RoomConnector.cs`

  Change room snapping                `RoomBuilder.cs`

  Change Auto Build rules             `LevelEditorSelection.cs`

  Change Auto Build Inspector         `LevelEditorSelectionEditor.cs`

  Add database rooms                  `RoomDatabase` asset

  Change room UI                      `RoomSelectedUI.cs` /
                                      `RoomButtonUI.cs`

  Change thumbnails                   `RoomThumbnailGenerator.cs`

  Change map saving                   `LevelMapSaver.cs`

  Change runtime NavMesh startup      `AutoGenerateAndBake.cs`

  Add interaction                     New `IInteractable` component

  Change interaction prompt           `InteractionPromptUI.cs`

  Change item definition              `ItemData.cs`

  Change item lookup                  `ItemDatabase.cs`

  Change inventory                    `Inventory.cs`

  Change equipment                    `EquipmentManager.cs` /
                                      `EquipmentSlot.cs`

  Add quest content                   `QuestData` asset

  Change quest progression            `QuestManager.cs`

  Change quest UI                     `QuestUI.cs`, `QuestMenuUI.cs`,
                                      `QuestDetailsUI.cs`

  Change enemy health                 `Enemy.cs`

  Change enemy movement               `EnemyAI.cs`

  Change enemy attacks                `EnemyCombat.cs`

  Change enemy spawning               `EnemySpawner.cs`

  Change player movement              `PlayerMovement.cs`

  Change player states                `PlayerStateMachine.cs`

  Change player attacks               `PlayerCombat.cs`

  Change player interaction           `PlayerInteraction.cs` only if the
                                      interaction framework itself
                                      changes

  Change saving                       `SaveData.cs` + `SaveManager.cs`

  Change scene loading                `SceneLoader.cs`
  -----------------------------------------------------------------------

------------------------------------------------------------------------

# 60. Final mental model

The most important thing to remember about this project is that the
systems are intentionally layered.

### Rooms

``` text
Room prefab
    ↓
RoomConnector
    ↓
RoomBuilder
    ↓
LevelEditorSelection
    ↓
RoomDatabase / UI / Auto Build
```

### Interaction

``` text
IInteractable
    ↓
PlayerInteraction
    ↓
InteractionUIManager
    ↓
InteractionPromptUI
```

### Items

``` text
ItemData
    ↓
ItemDatabase
    ↓
Inventory
    ↓
Equipment / UI / Save
```

### Quests

``` text
QuestData
    ↓
Quest
    ↓
QuestObjective
    ↓
QuestManager
    ↓
Quest UI
```

### Enemies

``` text
Enemy
├── EnemyAI
├── EnemyCombat
├── EnemyStateMachine
└── EnemySpawner
```

### Runtime generated navigation

``` text
LevelEditorSelection
    ↓
AutoGenerateAndBake
    ↓
NavMeshSurface
    ↓
EnemyAI / NavMeshAgent
```

If a new feature fits into one of these pipelines, extend the existing
extension point instead of bypassing it.

------------------------------------------------------------------------

# 61. Recommended next architectural improvements

These are recommendations based on the current uploaded code, not claims
that the features already exist.

## 1. Consolidate level-editor ownership

`LevelEditorManager` and `LevelEditorSelection` overlap in
responsibility. A future cleanup could make one the single source of
truth.

## 2. Create a formal room-generation rule system

As room rules grow, replace many conditions inside `TryBuildRoom()` with
a configurable rule system.

## 3. Add deterministic random seeds

This would make a generated map reproducible and would make debugging
much easier.

## 4. Add generation result/status

Instead of only `IsAutoBuildRunning`, expose a result such as:

``` text
Generating
Success
FailedMinimumRooms
FailedRequiredRooms
FailedStartingRoom
```

This would make runtime startup and UI feedback clearer.

## 5. Add a dedicated runtime room initializer

Use it after generation to populate enemies, chests, loot, events, and
other room content.

## 6. Standardize databases

If the project grows, consider making `ItemDatabase`, enemy databases,
quest databases, and room databases follow one consistent
ScriptableObject architecture.

------------------------------------------------------------------------

# 62. Summary

You can add most new content without rewriting the central systems:

-   **New room:** make a `Room` prefab, add connectors/bounds, add it to
    `RoomDatabase`, generate thumbnail.
-   **New interaction:** implement `IInteractable`.
-   **New item:** create `ItemData`, add it to `ItemDatabase`.
-   **New pickup:** use `ItemPickup` and assign the item.
-   **New quest:** create `QuestData`, configure objectives, assign to
    `QuestGiver`.
-   **New objective:** add an `ObjectiveType` and a corresponding
    `QuestManager` progress method.
-   **New enemy:** configure `Enemy`, `EnemyAI`, `EnemyCombat`,
    `EnemyStateMachine`, and spawn through `EnemySpawner`.
-   **New Auto Build rule:** modify `LevelEditorSelection`.
-   **New room selection behavior:** modify `LevelEditorSelectionEditor`
    / `RoomSelectedUI`.
-   **New thumbnail behavior:** modify `RoomThumbnailGenerator`.
-   **Runtime generated NavMesh:** `AutoGenerateAndBake` waits for Auto
    Build and then calls `NavMeshSurface.BuildNavMesh()`.

The most reusable extension point in the current project is
`IInteractable`; the most important generation abstraction is
`RoomBuilder`; and the main room-generation controller is
`LevelEditorSelection`.
