Noming-AVGC-XR

Third-person Action RPG prototype built in Unity 6.

Overview

Noming-AVGC-XR is an action RPG prototype focused on responsive third-person combat, enemy AI, boss encounters, procedural room-based level generation, interaction, inventory/equipment, quests, saving/loading, and modular gameplay systems.

The project has now reached an Initial Version milestone. The core gameplay and supporting systems are implemented and are being expanded and refined.

Current Development Status

Initial Version — Done

The first major version of the project is complete as a working prototype.

Current systems include:

Third-person player movement

Camera-relative movement

Sprinting

Jumping

Falling/gravity

Player state machine

Melee combat

Animation-driven weapon hit detection

Enemy AI

Enemy combat

Enemy state machine

Enemy spawning

Boss/multi-phase enemy architecture

NavMesh navigation

Runtime NavMesh generation/baking

Room-based level generation

Manual room placement

Automatic room generation

Room connectors

Room collision/overlap handling

Room databases

Required-room generation

Corridor generation

Dead-end generation

Auto-build rebuild attempts

Room selection/preview UI

Room thumbnails

Level map saving

Player interaction system

Interaction prompts

Inventory

Item database

Item pickups

Equipment system

Quest system

Quest data

Quest objectives

Quest giver

Quest UI

Save/load system

Main menu and scene loading

Minimap support

Player/enemy minimap indicators

Skill UI

Input System integration

Core Gameplay

Player

The player system includes:

CharacterController-based locomotion

Camera-relative movement

Walking

Sprinting

Jumping

Falling

Gravity

Ground detection

Player state machine

Attack state

Movement locking during attacks

Player health

Player UI

Main scripts:

Player.cs
PlayerMovement.cs
PlayerCombat.cs
PlayerStateMachine.cs
PlayerInteraction.cs
PlayerUI.cs
PlayerArrowUI.cs

Combat

The combat system is animation-driven.

Input
  ↓
PlayerCombat
  ↓
Animator
  ↓
Attack Animation
  ↓
Weapon Collider
  ↓
Weapon
  ↓
Enemy

Features:

Melee attacks

Attack cooldown

Attack state

Weapon hit detection

Animation-driven collider activation

Damage handling

Enemy death

Collision-based attacks

Main scripts:

PlayerCombat.cs
Weapon.cs
NormalAttackCollider.cs
PlayerCollusionDetector.cs
WeaponColliderBehaviourInAnimation.cs

Enemy AI

Enemies use NavMesh-based movement and state-driven behavior.

Features:

Enemy health

Enemy death

Enemy AI

Player detection

Chase behavior

Attack range

Melee attacks

Attack cooldown

Enemy state machine

Enemy spawning

Enemy minimap indicators

Main scripts:

Enemy.cs
EnemyAI.cs
EnemyCombat.cs
EnemyStateMachine.cs
EnemySpawner.cs
EnemyMiniMapDot.cs

Boss / Multi-Phase Combat

Enemy health supports phase-based encounters.

Features include:

Multiple health phases

Configurable phase health

Optional damage carry-over

Phase transitions

Normal attacks

Meteor attack

Death handling

The existing enemy/combat architecture can be extended for additional boss attacks and phase abilities.

Procedural Room System

The project contains a room-based level-generation system.

Core architecture:

RoomDatabase
      ↓
Room Prefab
      ↓
RoomConnector
      ↓
RoomBuilder
      ↓
LevelEditorSelection
      ↓
Generated Map

Room

Room.cs contains the room's metadata and connectors.

Room types include:

Start
Normal
DeadEnd
Boss
Treasure
Shop
Event
Stair
Secret

Room sizes include:

Small
Medium
Large
Corridor

Room Database

RoomDatabase.cs stores the available room prefabs.

This allows the level-generation system to select rooms without hard-coding individual prefabs into the generator.

Room Builder

RoomBuilder.cs handles connecting a new room to an existing connector.

The basic process is:

Select connector
      ↓
Select room prefab
      ↓
Find available connector
      ↓
Calculate rotation
      ↓
Align connector
      ↓
Snap room position
      ↓
Mark connectors occupied
      ↓
Store build history

Automatic Room Generation

The Level Editor supports automatic generation.

Auto Build supports:

Maximum room count

Minimum room count

Dead ends

Corridors

Corridor chain counting

Required rooms

Rebuild attempts

Collision/overlap checking

Auto-generated room tracking

Cleanup of previously auto-generated rooms

The generator can rebuild the generated section when required rooms are missing.

Starting Room

The generator uses the first room/starting room as the beginning of an Auto Build chain.

Auto-generated rooms are tracked so the generated map can be cleared and rebuilt without deleting unrelated/manual content.

Room Connectors

Connectors define where rooms can be attached.

A connector tracks:

Direction
Anchor Point
Occupied state
Connected Room

The runtime + selection indicator shows available connectors.

Free connector
    ↓
    +

Occupied connector
    ↓
    X / disabled

Room Selection and Editor Tools

The project contains custom editor tooling for the room-generation workflow.

Main scripts:

LevelEditorManager.cs
LevelEditorSelection.cs
LevelEditorSelectionEditor.cs
RoomButtonUI.cs
RoomSelectedUI.cs
RoomOutline.cs

These systems handle:

Room selection

Room preview

Connector selection

Manual room building

Auto Build

Required room selection

Room indicators

Editor camera controls

Room Thumbnails

Room thumbnails are generated using:

RoomThumbnailGenerator.cs

The thumbnail system creates temporary preview instances and renders them using a dedicated preview camera/layer.

General workflow:

Room prefab
    ↓
Temporary preview instance
    ↓
Preview layer
    ↓
Preview camera
    ↓
Calculate renderer bounds
    ↓
Frame camera
    ↓
RenderTexture
    ↓
PNG
    ↓
Room thumbnail

This keeps thumbnails separate from the actual runtime room instances.

Level Map Saving

The project contains:

LevelMapSaver.cs

The room-generation system stores enough information about generated rooms to support map saving/loading workflows.

Room build history is maintained by the Room component.

Runtime NavMesh

The project includes runtime NavMesh generation.

Main script:

AutoGenerateAndBake.cs

The intended order is:

Generate rooms
      ↓
Finish generation
      ↓
Sync transforms/physics
      ↓
Build NavMesh
      ↓
Enable systems that depend on navigation

This is important because enemies using NavMeshAgent cannot correctly navigate newly generated geometry until the NavMesh has been built.

Interaction System

The project contains a modular interaction system.

Main scripts:

IInteractable.cs
PlayerInteraction.cs
InteractionPromptUI.cs
InteractionUIManager.cs

The basic architecture is:

Player
  ↓
PlayerInteraction
  ↓
Find IInteractable
  ↓
InteractionPromptUI
  ↓
Player presses Interact
  ↓
IInteractable.Interact()

This allows different objects to implement their own interaction behavior without putting all interaction logic into the Player script.

Adding a New Interactable

The normal pattern is:

Create a GameObject.

Add the required collider.

Add a script implementing IInteractable.

Implement the interaction behavior.

Configure the interaction prompt if required.

Test it through the Player interaction system.

Example architecture:

Door
├── Mesh
├── Collider
└── DoorInteractable

The door should own door behavior; the player interaction system should only detect and invoke it.

Inventory System

The project contains:

Inventory.cs
InventorySlot.cs
InventoryUI.cs
ItemData.cs
ItemDatabase.cs
ItemPickup.cs
ItemPopupUI.cs
ItemVisual.cs

The architecture separates item data from runtime inventory state.

ItemData
    ↓
ItemDatabase
    ↓
ItemPickup
    ↓
Inventory
    ↓
InventoryUI

Adding a New Item

General workflow:

Create an ItemData asset.

Configure the item's properties.

Add it to the ItemDatabase.

Create/configure an item pickup if the item exists in the world.

Configure its visual representation.

Test pickup and inventory UI behavior.

This keeps item definitions data-driven instead of hard-coding every item into the inventory system.

Equipment System

Equipment is handled by:

EquipmentManager.cs
EquipmentSlot.cs

The system separates inventory items from equipped items.

General flow:

Inventory
   ↓
Select item
   ↓
EquipmentManager
   ↓
EquipmentSlot
   ↓
Equipped item

This allows the equipment system to be expanded with additional equipment slots and item categories.

Quest System

The project contains a complete quest-related structure:

Quest.cs
QuestData.cs
QuestObjective.cs
QuestObjectiveData.cs
QuestManager.cs
QuestGiver.cs
QuestButtonUI.cs
QuestDetailsUI.cs
QuestMenuUI.cs
QuestUI.cs

The architecture separates quest definitions from runtime quest state.

General flow:

QuestData
    ↓
Quest
    ↓
QuestManager
    ↓
Objectives
    ↓
Quest UI

Quest givers provide the player with access to quests, while the Quest Manager handles active quest state.

Adding a New Quest

General workflow:

Create/configure the quest data.

Add objectives.

Configure rewards/requirements as supported.

Assign the quest to the appropriate Quest Giver.

Configure the quest UI.

Test accepting, progressing and completing the quest.

The project also contains:

Assets/Editor/QuestGenerator.cs

for editor-side quest creation/generation support.

Save System

The project contains:

SaveData.cs
SaveManager.cs

The save system is responsible for serializing persistent game information.

The system can be expanded as new persistent gameplay systems are introduced.

When adding a new persistent system, its state should be added to the save data rather than relying on scene objects to reconstruct arbitrary runtime state.

UI Systems

The project contains UI systems for:

Player HUD

Inventory

Items

Equipment

Quests

Interaction prompts

Room selection

Skills

Main menu

Scene loading

Minimap

Important scripts include:

PlayerUI.cs
InventoryUI.cs
ItemPopupUI.cs
QuestUI.cs
QuestMenuUI.cs
QuestDetailsUI.cs
QuestButtonUI.cs
InteractionPromptUI.cs
InteractionUIManager.cs
SkillSlotUI.cs
MainMenuUIManager.cs
MenuManager.cs
SceneLoader.cs

Minimap

The project contains:

MinimapCamera.cs
PlayerArrowUI.cs
EnemyMiniMapDot.cs

The minimap architecture separates the minimap camera from the UI indicators used to represent gameplay entities.

Input System

The project uses Unity's Input System.

Main asset:

Assets/InputSystem_Actions.inputactions

The project includes a generated C# wrapper:

Assets/InputSystem_Actions.cs

Current player actions include movement, camera look, attack, interaction, crouch, jump, previous/next and sprint functionality.

When adding an input:

Add the action to the Input Actions asset.

Add bindings.

Save the Input Actions asset.

Let Unity regenerate the wrapper if required.

Subscribe to the action in the appropriate gameplay system.

Adding a New Gameplay System

When adding a new feature, keep responsibilities separated.

Recommended pattern:

Data
 ↓
Runtime System
 ↓
Component
 ↓
UI / Animation / Physics

For example:

ItemData
 ↓
Inventory
 ↓
Player
 ↓
InventoryUI

or:

RoomDatabase
 ↓
RoomBuilder
 ↓
Generated Room
 ↓
NavMesh
 ↓
Enemy AI

Avoid putting unrelated systems into Player.cs, Enemy.cs, or one giant manager.

Main Development Scripts

The major gameplay/editor scripts currently include:

AutoGenerateAndBake.cs
Enemy.cs
EnemyAI.cs
EnemyCombat.cs
EnemySpawner.cs
EnemyStateMachine.cs

Player.cs
PlayerMovement.cs
PlayerCombat.cs
PlayerStateMachine.cs
PlayerInteraction.cs

Weapon.cs

IInteractable.cs
InteractionPromptUI.cs
InteractionUIManager.cs

Inventory.cs
ItemData.cs
ItemDatabase.cs
ItemPickup.cs
EquipmentManager.cs
EquipmentSlot.cs

Quest.cs
QuestData.cs
QuestObjective.cs
QuestObjectiveData.cs
QuestManager.cs
QuestGiver.cs

Room.cs
RoomBuilder.cs
RoomConnector.cs
RoomDatabase.cs
RoomThumbnailGenerator.cs
LevelEditorManager.cs
LevelEditorSelection.cs
LevelEditorSelectionEditor.cs
LevelMapSaver.cs

Technology

Unity 6

C#

Unity Input System

CharacterController

NavMesh Agent

Unity AI Navigation

Animator State Machines

Animation Events

Cinemachine

Universal Render Pipeline

glTFast

Mixamo animations

Git

GitHub

Project Structure

The project currently contains the main Unity folders:

Assets/
Packages/
ProjectSettings/

The major custom systems are organized under:

Assets/Scripts/

Editor-only tools are under:

Assets/Editor/

Documentation

A detailed technical reference for the project is available in:

PROJECT_DOCUMENTATION.md

The documentation explains how the systems work and how to extend existing systems with new rooms, databases, interactions, items, quests, UI, enemies, and other features.

Planned / Future Features

The initial version is complete, but development continues.

Potential future work includes:

Gameplay

Stamina system

Enemy hit reactions

Additional boss abilities

Ranged combat

Magic abilities

More skills

More enemy types

More room types

More environmental interactions

RPG

Character progression

Experience system

Expanded loot system

More equipment

More item categories

More quest types

Dialogue expansion

UI

More HUD elements

Improved boss health UI

Skill hotbar improvements

Damage indicators

Improved inventory/equipment presentation

World

More room prefabs

More procedural generation rules

More map themes

More interactable objects

Expanded runtime NavMesh workflows

Development Workflow

The project uses Git and GitHub for version control.

Recommended workflow:

Make change
   ↓
Test in Unity
   ↓
Check Console
   ↓
Save project
   ↓
Review Git changes
   ↓
Commit
   ↓
Push

Avoid committing Unity-generated cache folders such as:

Library/
Temp/
Logs/
Obj/
UserSettings/

These should remain excluded by .gitignore.

Screenshots

Screenshots will be added as the visual presentation of the initial version is finalized.

Development Status

Initial Version — Complete

Core systems are implemented and connected.

The project is now in the expansion, refinement, balancing, content creation, and debugging stage.

Development is active.
