# Noming-AVGC-XR — Complete Project Documentation

**Unity version:** Unity `6000.3.15f1`

**Project type:** Third-person Action RPG prototype.

> **Important:** This document describes what is actually present in the uploaded repository. The repository README describes several systems that are planned or previously discussed, but some of those systems are **not present as C# implementations in this repository snapshot**. They are called out explicitly instead of being invented.

---

# 1. Project Overview

Noming-AVGC-XR is a Unity 6 third-person Action RPG prototype.

The current repository contains:

- Unity 6 project files
- Universal Render Pipeline configuration
- Unity Input System
- Cinemachine
- Unity AI Navigation / NavMesh
- glTFast
- character models and animation packs
- map/environment asset packs
- player movement
- player combat
- player state machine
- enemy health/phase handling
- enemy AI
- enemy combat
- enemy state machine
- meteor attack
- weapon hit detection
- camera look
- basic gravity helper
- two scenes
- two main Animator Controllers
- a generated Input System wrapper

The repository is currently much smaller at the gameplay-code level than the earlier `Scripts.zip`: the uploaded project contains **18 non-tutorial C# files**.

---

# 2. Repository Structure

```text
Noming-AVGC-XR-main/
│
├── Assets/
│   ├── Character/
│   │   ├── Characters/
│   │   │   ├── Animation/
│   │   │   └── Model/
│   │   ├── Enemy/
│   │   ├── MainCharacter/
│   │   └── Weapon/
│   │
│   ├── Map/
│   │   ├── kenney_fantasy-town-kit_2.0/
│   │   ├── kenney_food-kit/
│   │   ├── kenney_graveyard-kit_5.0/
│   │   ├── kenney_modular-dungeon-kit_1.0/
│   │   ├── kenney_prototype-kit/
│   │   └── kenney_retro-fantasy-kit/
│   │
│   ├── Scripts/
│   │   ├── CameraLook.cs
│   │   ├── Enemy.cs
│   │   ├── EnemyAI.cs
│   │   ├── EnemyCombat.cs
│   │   ├── EnemyStateMachine.cs
│   │   ├── Gravity.cs
│   │   ├── Meteor.cs
│   │   ├── NormalAttackCollider.cs
│   │   ├── Player.cs
│   │   ├── PlayerCollusionDetector.cs
│   │   ├── PlayerCombat.cs
│   │   ├── PlayerMovement.cs
│   │   ├── PlayerStateMachine.cs
│   │   └── Weapon.cs
│   │
│   ├── Settings/
│   ├── TutorialInfo/
│   ├── WeaponColliderBehaviourInAnimation.cs
│   └── InputSystem_Actions.inputactions
│
├── Packages/
├── ProjectSettings/
└── README.md
```

---

# 3. Unity Project Configuration

## 3.1 Unity version

The project is configured for:

```text
Unity 6000.3.15f1
```

Always open the project with the same Unity version or expect possible asset/package migration issues.

---

# 4. Installed Unity Packages

The project currently uses these important packages.

| Package | Version | Purpose |
|---|---:|---|
| Unity AI Navigation | 2.0.12 | NavMesh navigation |
| Cinemachine | 3.1.6 | Camera systems |
| glTFast | 6.19.0 | glTF/GLB import |
| Input System | 1.19.0 | Player input |
| URP | 17.3.0 | Rendering |
| Timeline | 1.8.12 | Timeline support |
| UGUI | 2.0.0 | UI |
| Visual Scripting | 1.9.11 | Visual scripting |
| Mathematics | 1.3.3 | Unity math support |
| Burst | 1.8.29 | Burst compilation |

The package configuration is in:

```text
Packages/manifest.json
Packages/packages-lock.json
```

---


## 5.1 MainGame

`MainGame.unity` is the primary gameplay scene in the repository.

Important GameObjects include:

```text
Boss
FreeLook Camera
Main Camera
Directional Light
Meteor
Global Volume
Basic Attack Collider
Ground Check
Map
NormalAttackCollider
Character
GoblinMob
Camera Target
Weapon Collider
```

The main gameplay scripts are attached approximately as follows:

```text
Character
├── Player
├── PlayerMovement
├── PlayerCombat
└── PlayerStateMachine

Camera Target
└── CameraLook

Weapon Collider
└── Weapon

GoblinMob
├── Enemy
├── EnemyAI
├── EnemyStateMachine
└── EnemyCombat

Boss
├── Enemy
├── EnemyAI
├── EnemyStateMachine
└── EnemyCombat

Basic Attack Collider
└── NormalAttackCollider

Meteor
└── Meteor
```

There are also Unity/Cinemachine/URP components in the scene that are not custom scripts.

## 5.2 SampleScene

`Assets/Scenes/SampleScene.unity` is a simple sample scene containing:

```text
Main Camera
Directional Light
Global Volume
```

The Build Settings currently list:

```text
Assets/Scenes/SampleScene.unity
```

as the enabled scene.

If `MainGame.unity` is intended to be the actual startup scene, add it to Build Settings and place it before or instead of SampleScene.

---

# 6. Input System

The input asset is:

```text
Assets/InputSystem_Actions.inputactions
```

Unity has generated:

```text
Assets/InputSystem_Actions.cs
```

The project has a `Player` action map.

Current actions include:

```text
Move
Look
Attack
Interact
Crouch
Jump
Previous
Next
Sprint
```

## 6.1 Current important bindings

### Move

Supports:

- WASD
- Arrow keys
- Gamepad left stick
- XR primary 2D axis
- Joystick

### Look

Supports:

- Mouse/pointer delta
- Gamepad right stick
- Joystick hat switch

### Attack

Supports:

- Mouse left button
- Gamepad
- Touch
- Joystick trigger
- XR primary action

### Interact

The action exists in the Input System asset and is configured as a button with a Hold interaction.

**Important:** in this repository snapshot, there is no gameplay `PlayerInteraction.cs` implementation using this action.

So the input action exists, but the complete interaction system described in the README is not currently implemented in the uploaded source.

---

# 7. Player Architecture

The player is split into four main gameplay scripts:

```text
Player
│
├── PlayerMovement
├── PlayerCombat
└── PlayerStateMachine
```

The overall flow is:

```text
Input System
      │
      ├── Move ───────→ PlayerMovement
      ├── Jump ───────→ PlayerMovement
      ├── Sprint ─────→ PlayerMovement
      └── Attack ─────→ PlayerCombat
                              │
                              ↓
                           Animator
                              │
                              ↓
                    Weapon collider timing
                              │
                              ↓
                           Weapon
                              │
                              ↓
                            Enemy
```

---

# 8. Player.cs

Location:

```text
Assets/Scripts/Player.cs
```

## Responsibility

`Player` owns the player's health and basic death state.

Inspector fields:

```csharp
maxHealth
attackDamage
```

Runtime properties:

```csharp
IsDead
HealthPercent
AttackDamage
```

## Initialization

During `Awake()`:

1. Finds the Animator on the same GameObject.
2. Sets current health to max health.

## Taking damage

```csharp
TakeDamage(int damage)
```

reduces health.

If health reaches zero:

```text
TakeDamage
   ↓
currentHealth <= 0
   ↓
Die()
   ↓
IsDead = true
   ↓
Animator.SetTrigger("Dead")
```

## Adding player stats

For example, to add defense:

```csharp
[SerializeField]
private int defense = 10;
```

Then modify damage:

```csharp
int finalDamage =
    Mathf.Max(
        0,
        damage - defense);
```

Keep the actual health/damage responsibility inside `Player` rather than putting it in `PlayerMovement`.

---

# 9. PlayerMovement.cs

Location:

```text
Assets/Scripts/PlayerMovement.cs
```

## Responsibility

Handles:

- CharacterController movement
- camera-relative movement
- rotation
- sprinting
- jumping
- gravity
- falling detection
- movement lock during attacks
- movement state detection

## Required components

The player needs:

```text
CharacterController
Player
PlayerCombat
PlayerStateMachine
```

It also requires:

```text
Ground Check Transform
Camera Transform
Ground Layer
```

## Movement flow

```text
Input Move
   ↓
moveInput
   ↓
cameraForward / cameraRight
   ↓
moveDirection
   ↓
CharacterController.Move()
```

The movement direction is based on the camera rather than world axes.

## Rotation

When the player is moving:

```text
moveDirection
      ↓
Quaternion.LookRotation
      ↓
Slerp player rotation
```

This makes the character face the direction of movement.

## Sprint

The sprint action changes:

```text
currentSpeedMultiplier
```

The current implementation increases the multiplier when Sprint is performed and decreases it when Sprint is canceled.

### Important extension warning

If Sprint can be enabled/disabled repeatedly or the input object is recreated, a boolean-driven speed calculation is safer than repeatedly adding/subtracting the multiplier.

---

# 10. Jump and Gravity

Jump is handled in `PlayerMovement`.

Jump velocity is calculated using:

```text
sqrt(jumpHeight * -2 * gravity)
```

Gravity is then applied every frame.

There are separate upward/falling behaviors:

```text
velocity.y >= 0
    → gravity

velocity.y < 0
    → gravity * fallMultiplier
```

This makes falling faster than rising.

---

# 11. Ground Check

The player uses:

```csharp
Physics.CheckSphere()
```

with:

```text
groundCheck
groundDistance
groundLayer
```

The check is:

```text
groundCheck position
        +
sphere radius
        +
ground layer mask
```

If something on the selected Ground Layer is inside the sphere:

```text
isGrounded = true
```

### To add another ground type

Do not modify the script.

Instead:

1. Create/select the layer.
2. Put the floor object on that layer.
3. Add the layer to `groundLayer`.

---

# 12. Player State Machine

Location:

```text
Assets/Scripts/PlayerStateMachine.cs
```

Current states:

```text
Idle
Walking
Sprinting
Jump
Fall
```

The state machine uses:

```csharp
Dictionary<PlayerState, Action>
```

to map each state to a handler.

## State flow

`PlayerMovement` determines the state:

```text
Falling
   ↓
Fall

Jumping
   ↓
Jump

Sprint + movement
   ↓
Sprinting

Movement
   ↓
Walking

No movement
   ↓
Idle
```

Then:

```text
PlayerStateMachine.ChangeState()
        ↓
CurrentState
        ↓
stateActions[CurrentState]
        ↓
Animator.SetBool(...)
```

The Animator parameters are therefore expected to match:

```text
Idle
Walking
Sprinting
Jump
Fall
```

---

# 13. Adding a New Player State

Example: adding `Crouch`.

## Step 1 — Add enum value

In `PlayerStateMachine`:

```csharp
public enum PlayerState
{
    Idle,
    Walking,
    Sprinting,
    Jump,
    Fall,
    Crouch
}
```

## Step 2 — Add handler

```csharp
private void HandleCrouch()
{
    AnimationStateSetter(
        PlayerState.Crouch);
}
```

## Step 3 — Add dictionary entry

```csharp
{
    PlayerState.Crouch,
    HandleCrouch
}
```

## Step 4 — Add transition logic

In `PlayerMovement`, detect the Crouch input.

## Step 5 — Add Animator parameter

Create:

```text
Crouch
```

as a Bool parameter.

## Step 6 — Add Animator transitions

Configure:

```text
Idle → Crouch
Walking → Crouch
Crouch → Idle
Crouch → Walking
```

---

# 14. PlayerCombat.cs

Location:

```text
Assets/Scripts/PlayerCombat.cs
```

## Responsibility

Handles:

- attack input
- attack cooldown
- attack state
- Animator attack trigger
- movement lock during attack

## Attack flow

```text
Attack input
     ↓
attackPressed
     ↓
cooldown check
     ↓
isGrounded check
     ↓
isAttacking = true
     ↓
Animator.SetTrigger("Attack")
     ↓
animation
     ↓
EndAttack()
     ↓
isAttacking = false
```

## Movement lock

`PlayerMovement` checks:

```csharp
playerCombat.isAttacking
```

and prevents movement while attacking.

This is a good example of a simple cross-system dependency:

```text
PlayerCombat
     ↓
isAttacking
     ↓
PlayerMovement
```

---

# 15. Weapon System

There are two relevant scripts:

```text
Weapon.cs
WeaponColliderBehaviourInAnimation.cs
```

## Weapon.cs

The weapon:

1. Finds the Player in its parent.
2. Keeps a `HashSet<Enemy>` of enemies already hit during the current attack.
3. Detects trigger collisions.
4. Applies player attack damage.

The HashSet prevents one attack animation from damaging the same enemy repeatedly as the weapon remains inside the enemy collider.

## Resetting attack hits

```csharp
ResetHitEnemies()
```

is called at the beginning of an attack animation.

---

# 16. Animation-Driven Weapon Collider

`WeaponColliderBehaviourInAnimation` is a `StateMachineBehaviour`.

It enables the weapon collider when the attack animation state begins:

```text
OnStateEnter
    ↓
Find Weapon
    ↓
ResetHitEnemies
    ↓
weaponCollider.enabled = true
```

When the animation leaves:

```text
OnStateExit
    ↓
weaponCollider.enabled = false
```

This means the weapon collider is active only during the relevant animation state.

---

# 17. Adding a New Player Attack

The existing pattern should be reused.

Recommended workflow:

```text
Input
 ↓
PlayerCombat
 ↓
Animator Trigger
 ↓
Attack Animation
 ↓
StateMachineBehaviour
 ↓
Weapon Collider
 ↓
Weapon.OnTriggerEnter
 ↓
Enemy.TakeDamage
```

## Step 1

Add an input action if a separate button is required.

## Step 2

Add an Animator Trigger, for example:

```text
HeavyAttack
```

## Step 3

Add the animation.

## Step 4

Add the weapon collider behavior to the animation state.

## Step 5

Trigger it from `PlayerCombat`.

Example:

```csharp
animator.SetTrigger("HeavyAttack");
```

## Step 6

Tune damage/cooldown as needed.

---

# 18. Enemy Architecture

The enemy is divided into:

```text
Enemy
EnemyAI
EnemyCombat
EnemyStateMachine
```

The architecture is:

```text
Enemy
 │
 ├── Health / Death / Phases
 │
 ├── EnemyAI
 │      │
 │      ├── Detection
 │      ├── Chase
 │      └── Attack decision
 │
 ├── EnemyCombat
 │      │
 │      ├── Cooldown
 │      ├── Attack range
 │      ├── Normal attack
 │      └── Meteor attack
 │
 └── EnemyStateMachine
        │
        ├── Idle
        └── Sprinting
```

---

# 19. Enemy.cs

Location:

```text
Assets/Scripts/Enemy.cs
```

## Responsibility

Handles:

- health
- damage
- multi-phase health
- carry-over damage
- death
- death animation
- despawning

The Inspector exposes phase health.

The logic is:

```text
Damage
 ↓
currentHealth <= 0
 ↓
NextPhase()
 ↓
more phases?
 ├── Yes → next phase
 └── No → Die()
```

## Phase system

The `phaseHealth` array determines phase maximum health.

Example:

```text
phaseHealth:
    500
    800
    1200
```

means:

```text
Phase 1 → 500 HP
Phase 2 → 800 HP
Phase 3 → 1200 HP
```

If carry-over damage is enabled, excess damage from one phase is applied to the next phase.

---

# 20. Adding a Boss Phase

The current `Enemy` class can be used for phase-based bosses.

Example:

```text
Phase 1
Normal attack
      ↓
Health reaches 0
      ↓
Phase 2
Meteor unlocked
      ↓
Health reaches 0
      ↓
Phase 3
Final attack pattern
      ↓
Death
```

The intended place for phase-specific behavior is:

```csharp
EnterPhase(int phase)
```

Add phase logic there.

Example:

```csharp
case 1:
    // Enable phase 2 attack
    break;
```

---

# 21. EnemyAI.cs

Location:

```text
Assets/Scripts/EnemyAI.cs
```

## Responsibility

Handles:

- finding Player
- measuring distance
- deciding whether to attack
- chasing
- idling
- NavMeshAgent movement

Current decision tree:

```text
Enemy dead?
 └── stop

Enemy attacking?
 └── stop

Distance <= AttackRange?
 └── attack

Distance <= ChaseRange?
 └── chase

Otherwise
 └── idle
```

---

# 22. Enemy Chase

When chasing:

```csharp
agent.isStopped = false;
agent.SetDestination(player.position);
```

The state machine is switched to:

```text
Sprinting
```

This means the enemy Animator can use the same conceptual state name to drive its locomotion animation.

---

# 23. EnemyCombat.cs

Current attack types:

```text
Normal
Meteor
```

## Normal attack

The normal attack:

1. Checks `CanAttack()`.
2. Enables `normalAttackCollider`.
3. Sets `IsAttacking`.
4. Starts cooldown.
5. Triggers Animator `"Attack"`.

## Meteor attack

The meteor attack:

1. Checks `CanAttack()`.
2. Moves the meteor object above the player's position.
3. Activates it.
4. Sets `IsAttacking`.
5. Starts cooldown.
6. Triggers Animator `"Meteor"`.

---

# 24. NormalAttackCollider.cs

This collider belongs to the enemy attack system.

When it enters a Player trigger:

```text
OnTriggerEnter
 ↓
Player tag?
 ↓
Get Player
 ↓
Player.TakeDamage(enemy.AttackDamage)
```

The current script expects the enemy's attack damage to be exposed through `Enemy.AttackDamage`.

If this property is not present in the exact source state, this is an integration point that should be checked before changing the damage pipeline.

---

# 25. Meteor.cs

The meteor uses a delayed fall.

Flow:

```text
OnEnable
 ↓
canFall = false
 ↓
wait fallDelay
 ↓
canFall = true
 ↓
move downward
 ↓
below Y=0
 ↓
SetActive(false)
```

This is a simple reusable projectile-like hazard.

---

# 26. PlayerCollusionDetector.cs

The script name contains the current project's spelling:

```text
PlayerCollusionDetector
```

It detects collision/trigger entry with the Player and applies enemy attack damage.

It is conceptually another enemy contact-damage component.

### Recommended future cleanup

Rename it to:

```text
PlayerCollisionDetector
```

if you decide to standardize naming. If renamed, rename the `.cs` file and class together so Unity does not lose the component reference.

---

# 27. EnemyStateMachine.cs

Current enemy states are:

```text
Idle
Sprinting
```

It uses the same general pattern as the player:

```text
CurrentState
    ↓
Dictionary<State, Action>
    ↓
state handler
    ↓
Animator bools
```

The Animator parameters expected are:

```text
Idle
Sprinting
```

---

# 28. CameraLook.cs

Location:

```text
Assets/Scripts/CameraLook.cs
```

Handles mouse/gamepad look input.

It maintains:

```text
yaw
pitch
```

Pitch is clamped:

```text
-30° to 70°
```

The camera rotation becomes:

```text
Quaternion.Euler(
    pitch,
    yaw,
    0)
```

---

# 29. Cinemachine

The project contains Cinemachine 3.1.6.

`MainGame` includes:

```text
FreeLook Camera
Main Camera
Camera Target
```

The project also has a custom `CameraLook` script.

When modifying the camera, first determine whether the behavior is controlled by:

```text
Cinemachine
```

or:

```text
CameraLook
```

Do not implement the same rotation logic in both systems.

---

# 30. Map System in This Repository

The repository contains a `Map` GameObject in `MainGame`.

It also contains several environment asset packs:

```text
kenney_fantasy-town-kit_2.0
kenney_food-kit
kenney_graveyard-kit_5.0
kenney_modular-dungeon-kit_1.0
kenney_prototype-kit
kenney_retro-fantasy-kit
```

However:

> The uploaded repository does **not** contain the previously discussed `Room`, `RoomConnector`, `RoomBuilder`, `RoomDatabase`, `LevelEditorSelection`, thumbnail generator, or procedural room-builder scripts.

Those belong to the other script snapshot/conversation system and should not be treated as part of this repository unless they are added to this project.

---

# 31. NavMesh

The project includes:

```text
com.unity.ai.navigation
```

version:

```text
2.0.12
```

The enemy uses:

```csharp
UnityEngine.AI.NavMeshAgent
```

so enemy movement depends on a valid baked NavMesh.

The current uploaded project does not contain the previously discussed `AutoGenerateAndBake.cs`.

If runtime-generated maps are later added, the intended architecture should be:

```text
Generate world
      ↓
Wait for generation
      ↓
Physics.SyncTransforms()
      ↓
NavMeshSurface.BuildNavMesh()
      ↓
Spawn / enable AI
```

---

# 32. Adding a New Enemy

Use the existing enemy architecture.

## Step 1 — Duplicate the enemy

Create a new enemy GameObject/prefab.

Required conceptual components:

```text
Enemy
EnemyAI
EnemyCombat
EnemyStateMachine
Animator
NavMeshAgent
Collider(s)
```

## Step 2 — Configure Enemy

Set:

```text
Phase Health
Carry-over damage
Despawn delay
```

## Step 3 — Configure EnemyAI

Set:

```text
Chase Range
```

## Step 4 — Configure EnemyCombat

Set:

```text
Damage
Attack Cooldown
Attack Range
Normal Attack Collider
Meteor object if used
```

## Step 5 — Configure Animator

Make sure the parameters used by the state machine exist:

```text
Idle
Sprinting
Attack
Meteor
Die
```

depending on the enemy.

---

# 33. Adding a New Enemy Attack

Current pattern:

```text
EnemyAI
    ↓
EnemyCombat.Attack(type)
    ↓
AttackType
    ↓
Animator
    ↓
Collider / projectile
```

To add a new attack:

## Step 1

Add an enum value:

```csharp
public enum AttackType
{
    Normal,
    Meteor,
    NewAttack
}
```

## Step 2

Add the attack logic in:

```csharp
EnemyCombat.Attack()
```

## Step 3

Add an Animator trigger.

## Step 4

Create the attack animation.

## Step 5

Create the collider/projectile if needed.

## Step 6

Use animation events if damage timing needs to match a specific animation frame.

---

# 34. Adding a New Projectile/Hazard

The `Meteor` script is a useful template.

Basic pattern:

```text
OnEnable
 ↓
Reset state
 ↓
Wait / prepare
 ↓
Move
 ↓
Detect impact / boundary
 ↓
Disable
```

To create a fireball:

```text
Fireball.cs
```

could use:

```csharp
private void OnEnable()
{
    // Reset projectile
}

private void Update()
{
    // Move
}

private void OnTriggerEnter(Collider other)
{
    // Apply damage
}
```

Keep projectile logic in the projectile rather than inside the enemy AI.

---

# 35. Interaction System — Current Repository Status

The README says the project has:

- dynamic interaction prompts
- Input System integration
- automatic keybind display
- item interactions
- extensible interaction interface
- multiple nearby interactables

However, the uploaded repository currently contains only the `Interact` action in the Input System asset and does not contain the corresponding interaction interface/manager scripts.

Therefore the system should currently be considered:

```text
Input Action exists
        ↓
Gameplay interaction implementation
        ↓
NOT PRESENT in this repository snapshot
```

---

# 36. How to Add an Interaction System

If you want to implement it in this project, use an interface-based design.

Recommended:

```csharp
public interface IInteractable
{
    string GetInteractionText();

    void Interact(Player player);
}
```

Then a chest could implement:

```csharp
public class Chest : MonoBehaviour, IInteractable
{
    public string GetInteractionText()
    {
        return "Open Chest";
    }

    public void Interact(Player player)
    {
        // Open chest
    }
}
```

The player interaction system should:

```text
Detect interactable
      ↓
Store current target
      ↓
Show prompt
      ↓
InputSystem Interact
      ↓
IInteractable.Interact()
```

This keeps interactables independent.

---

# 37. Adding a New Interactable

Recommended workflow:

```text
1. Create GameObject
2. Add Collider
3. Add script implementing IInteractable
4. Add interaction detection
5. Add UI prompt
6. Configure input
```

Example:

```text
Door
├── Mesh
├── Collider
└── DoorInteractable
```

The Door script should not need to know how the UI works.

---

# 38. Inventory / Equipment / Database Systems

The README describes:

- Inventory
- Inventory UI
- Equipment
- Item Database
- Equipment slots
- item inspection
- equipment swapping

But these systems are **not present as gameplay C# scripts in the uploaded repository snapshot**.

Therefore there is no current project implementation from which to document exact classes, fields, or inspector setup.

If these systems are added later, use a data-driven structure:

```text
ItemData
    ↓
ItemDatabase
    ↓
Inventory
    ↓
Equipment
    ↓
UI
```

---

# 39. Creating a New Database

For future data-driven systems, prefer ScriptableObjects for static data.

Example:

```csharp
[CreateAssetMenu(
    menuName = "RPG/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int value;
}
```

Then a database:

```csharp
[CreateAssetMenu(
    menuName = "RPG/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;
}
```

This gives:

```text
Assets
└── Data
    ├── Items
    │   ├── Sword.asset
    │   ├── Potion.asset
    │   └── Shield.asset
    │
    └── ItemDatabase.asset
```

This is a recommended future architecture, not a claim about the current repository.

---

# 40. Thumbnails

The uploaded repository does not contain the previously discussed `RoomThumbnailGenerator`.

Therefore there is currently no room-database thumbnail workflow in this project snapshot.

If a room database is introduced, a good thumbnail pipeline is:

```text
Room prefab
    ↓
Temporary preview instance
    ↓
Preview layer
    ↓
Temporary camera
    ↓
Calculate renderer bounds
    ↓
Frame camera
    ↓
RenderTexture
    ↓
Texture2D
    ↓
Encode PNG
    ↓
Assets/Room Thumbnails/
```

A thumbnail generator should be an Editor-only script:

```csharp
#if UNITY_EDITOR
...
#endif
```

This prevents Editor APIs from entering runtime builds.

---

# 41. Adding a New Room to a Future Room System

If the procedural room system from the other development branch is merged into this project, the expected workflow is:

```text
Create room model
    ↓
Create prefab
    ↓
Add Room component
    ↓
Add RoomBounds
    ↓
Add RoomConnector objects
    ↓
Configure connector directions
    ↓
Add prefab to RoomDatabase
    ↓
Generate thumbnail
    ↓
Test manual placement
    ↓
Test Auto Build
```

Do not add room prefabs directly to procedural generation without configuring connectors and bounds.

---

# 42. Animation Architecture

There are two main Animator Controllers:

```text
Assets/Character/PlayerAnimation.controller
Assets/Character/Enemy/Goblin.controller
```

## Player Animator

The player state machine controls bool parameters corresponding to:

```text
Idle
Walking
Sprinting
Jump
Fall
```

Combat uses:

```text
Attack
Dead
```

depending on the controller configuration.

## Enemy Animator

The enemy state machine uses:

```text
Idle
Sprinting
```

and combat/death uses additional triggers.

---

# 43. Animation Events

The project uses animation events for gameplay timing.

Examples:

```text
PlayerCombat.EndAttack()
```

and the enemy attack completion flow.

The weapon collider is also tied directly to animation state entry/exit.

### Rule

If an action must happen at an exact animation frame, prefer:

```text
Animation Event
```

or:

```text
StateMachineBehaviour
```

rather than timing it with arbitrary `Update()` delays.

---

# 44. Adding an Animation Event

Example:

```text
Attack animation
        ↓
Frame 20
        ↓
EnableWeaponCollider()
        ↓
Frame 30
        ↓
DisableWeaponCollider()
```

The target GameObject must contain the component containing the event method.

The method should be:

```csharp
public void MethodName()
{
}
```

Unity Animation Events require an accessible event method.

---

# 45. Tags

The current TagManager includes:

```text
Weapon
Weapon Collider
```

and:

```text
Player
```

is configured as a layer.

The Player detection code uses:

```csharp
CompareTag("Player")
```

The Player GameObject therefore needs the `Player` tag if the collision scripts are expected to detect it.

---

# 46. Weapon Collider Tag

`PlayerCombat` currently finds the weapon collider with:

```csharp
GameObject.FindGameObjectWithTag(
    "Weapon Collider");
```

Therefore the weapon collider object must have:

```text
Tag = Weapon Collider
```

### Important architecture note

This is convenient for a prototype but fragile for a larger project.

A better long-term pattern is:

```text
[SerializeField]
private GameObject weaponCollider;
```

and assign it in the Inspector.

This avoids global scene searches and allows multiple players/weapons.

---

# 47. Adding a New Player Weapon

Current architecture:

```text
Character
└── Weapon Collider
    └── Weapon
```

The `Weapon` finds the Player in its parent hierarchy.

To add another weapon:

1. Duplicate/create weapon model.
2. Add `Weapon`.
3. Add Collider.
4. Set collider as Trigger if required.
5. Put it under the player/weapon hierarchy.
6. Configure the animation collider behavior.
7. Make sure the appropriate animation enables/disables it.

---

# 48. Save System — Current Repository Status

The README describes:

- JSON save files
- Save & Load
- Continue Game
- Auto Save
- Save deletion
- save detection
- main menu integration

But the uploaded repository does not contain a gameplay `SaveManager.cs` or equivalent save implementation.

Therefore these should be treated as README-level/planned functionality, not documented as currently implemented code.

---

# 49. Quest System — Current Repository Status

The README lists:

- Quest framework
- Dialogue
- NPC interactions
- world persistence

but no quest implementation scripts are present in the uploaded project's gameplay source.

Do not create references to nonexistent classes such as `QuestManager` until they are actually added to this repository.

---

# 50. Adding a Future Quest System

Recommended structure:

```text
QuestData
    ↓
QuestManager
    ↓
Quest State
    ↓
Objectives
    ↓
UI
```

Use ScriptableObjects for static quest definitions.

Example:

```text
QuestData
├── ID
├── Display Name
├── Description
├── Objectives
└── Rewards
```

Runtime state should be separate from the static definition.

---

# 51. Debugging Workflow

When something does not work, use this order.

## 51.1 Input problem

Check:

```text
InputSystem_Actions.inputactions
        ↓
Action exists
        ↓
Binding exists
        ↓
Input action map enabled
        ↓
Script subscribes to action
```

## 51.2 Player movement problem

Check:

```text
CharacterController
Ground Check
Ground Layer
Camera Transform
PlayerMovement
PlayerCombat
```

## 51.3 Attack problem

Check:

```text
Attack binding
Animator Attack trigger
Attack animation
isGrounded
attackCooldown
isAttacking
weapon collider
Weapon script
Enemy collider
```

## 51.4 Enemy not moving

Check:

```text
NavMesh baked
NavMeshAgent
EnemyAI
Player tag
Player exists
Chase Range
Attack Range
```

## 51.5 Enemy not damaging player

Check:

```text
Attack collider
Is Trigger
Player tag
Player component
Enemy reference
Enemy attack damage
Animator timing
```

---

# 52. Common Dependency Problems

## Missing component

Many scripts use:

```csharp
GetComponent<T>()
```

For example:

```text
PlayerMovement → Player
PlayerMovement → PlayerCombat
PlayerMovement → PlayerStateMachine
EnemyAI → Enemy
EnemyAI → EnemyCombat
EnemyAI → EnemyStateMachine
EnemyCombat → Enemy
```

If one of those components is absent, the system can produce null-reference errors.

---

# 53. Avoiding NullReferenceException

When adding a new dependency:

```csharp
private MySystem system;
```

initialize it in `Awake()`:

```csharp
system =
    GetComponent<MySystem>();
```

For Inspector references:

```csharp
[SerializeField]
private Transform target;
```

verify the field is assigned.

For scene lookups:

```csharp
GameObject.FindGameObjectWithTag(...)
```

verify:

- object exists
- tag exists
- object has the expected component

---

# 54. How to Add a Completely New System

Use this pattern:

## Step 1 — Define responsibility

Ask:

> What single thing should this system own?

For example:

```text
DoorSystem → doors
QuestSystem → quests
LootSystem → loot
DialogueSystem → dialogue
```

## Step 2 — Create the data layer

For static data:

```text
ScriptableObject
```

## Step 3 — Create runtime logic

Use:

```text
MonoBehaviour
```

for scene/runtime behavior.

## Step 4 — Connect through small interfaces/properties

Avoid making every system directly access every other system.

Bad:

```text
Player → Enemy → UI → Inventory → Quest → Camera
```

Better:

```text
Player
 ↓
Combat

Player
 ↓
Interaction
 ↓
Interactable

QuestManager
 ↓
QuestUI
```

---

# 55. Recommended Project Organization as It Grows

The current project can be reorganized toward:

```text
Assets/
│
├── Scripts/
│   ├── Core/
│   │
│   ├── Player/
│   │   ├── Movement/
│   │   ├── Combat/
│   │   └── States/
│   │
│   ├── Enemy/
│   │   ├── AI/
│   │   ├── Combat/
│   │   └── States/
│   │
│   ├── Interaction/
│   ├── Inventory/
│   ├── Equipment/
│   ├── Quest/
│   ├── Dialogue/
│   ├── Save/
│   ├── UI/
│   └── World/
│
├── Data/
│   ├── Items/
│   ├── Quests/
│   ├── Enemies/
│   └── Rooms/
│
├── Prefabs/
│   ├── Player/
│   ├── Enemies/
│   ├── Weapons/
│   ├── Rooms/
│   └── Interactables/
│
├── Scenes/
├── Animations/
├── Materials/
└── UI/
```

This is a recommended future organization, not the current repository layout.

---

# 56. Current Script Reference

## `CameraLook.cs`

```text
Assets/Scripts/CameraLook.cs
```

Purpose:

```text
Camera rotation from Input System Look action.
```

---

## `Enemy.cs`

```text
Assets/Scripts/Enemy.cs
```

Purpose:

```text
Enemy health, phases, damage and death.
```

---

## `EnemyAI.cs`

```text
Assets/Scripts/EnemyAI.cs
```

Purpose:

```text
Player detection, chase and attack decision.
```

---

## `EnemyCombat.cs`

```text
Assets/Scripts/EnemyCombat.cs
```

Purpose:

```text
Enemy attack cooldown and attack execution.
```

---

## `EnemyStateMachine.cs`

```text
Assets/Scripts/EnemyStateMachine.cs
```

Purpose:

```text
Enemy locomotion state → Animator.
```

---

## `Gravity.cs`

```text
Assets/Scripts/Gravity.cs
```

Purpose:

```text
Simple constant downward movement.
```

---

## `Meteor.cs`

```text
Assets/Scripts/Meteor.cs
```

Purpose:

```text
Delayed falling meteor hazard.
```

---

## `NormalAttackCollider.cs`

```text
Assets/Scripts/NormalAttackCollider.cs
```

Purpose:

```text
Enemy normal attack collision damage.
```

---

## `Player.cs`

```text
Assets/Scripts/Player.cs
```

Purpose:

```text
Player health and death.
```

---

## `PlayerCollusionDetector.cs`

```text
Assets/Scripts/PlayerCollusionDetector.cs
```

Purpose:

```text
Trigger-based player damage detection.
```

---

## `PlayerCombat.cs`

```text
Assets/Scripts/PlayerCombat.cs
```

Purpose:

```text
Player attack input, cooldown and attack state.
```

---

## `PlayerMovement.cs`

```text
Assets/Scripts/PlayerMovement.cs
```

Purpose:

```text
Movement, sprint, jump, gravity and movement states.
```

---

## `PlayerStateMachine.cs`

```text
Assets/Scripts/PlayerStateMachine.cs
```

Purpose:

```text
Player state → Animator.
```

---

## `Weapon.cs`

```text
Assets/Scripts/Weapon.cs
```

Purpose:

```text
Weapon hit detection and damage.
```

---

## `WeaponColliderBehaviourInAnimation.cs`

```text
Assets/WeaponColliderBehaviourInAnimation.cs
```

Purpose:

```text
Animation-driven weapon collider activation.
```

---

# 57. Tutorial Scripts

The repository contains:

```text
Assets/TutorialInfo/
```

including:

```text
Readme.cs
ReadmeEditor.cs
```

These are Unity template/tutorial support assets rather than gameplay architecture.

They should generally not be used as dependencies for RPG systems.

---

# 58. How to Add a New Script Safely

1. Decide which system owns the behavior.
2. Put the script in that system's folder.
3. Give it one clear responsibility.
4. Add serialized references only for things that genuinely need Inspector assignment.
5. Use `GetComponent` for same-object dependencies.
6. Avoid `GameObject.Find` unless there is a strong reason.
7. Add required Animator parameters before testing animation code.
8. Test the smallest feature first.
9. Check Console for null references.
10. Test death/disable/re-enable cases.

---

# 59. How to Add a New Database

If the new system uses static designer-authored data:

### Recommended

```text
ScriptableObject
```

Example:

```text
Assets/Data/
    EnemyDatabase.asset
```

Use:

```csharp
[CreateAssetMenu]
```

so Unity provides:

```text
Right Click
 → Create
 → RPG
 → Enemy Database
```

Then reference the database from a manager.

---

# 60. How to Add a New UI System

Recommended dependency direction:

```text
Gameplay system
      ↓
Public state/data
      ↓
UI
```

Avoid:

```text
PlayerMovement
    ↓
GetComponent<Text>()
```

Instead expose:

```csharp
public float HealthPercent
```

and let UI read it.

The existing `Player` already demonstrates this pattern with:

```text
HealthPercent
```

---

# 61. How to Add a Health Bar

Use the existing:

```csharp
Player.HealthPercent
```

Create:

```text
HealthBar
└── Slider
```

Then:

```csharp
slider.value =
    player.HealthPercent;
```

The Player remains responsible for health; the UI remains responsible for display.

---

# 62. How to Add Enemy Health UI

The current `Enemy` already exposes:

```text
GetCurrentHealth()
GetCurrentPhaseMaxHealth()
```

A UI component can calculate:

```text
current health / phase maximum health
```

without modifying the enemy's internal health logic.

---

# 63. Future Runtime Map Generation

If procedural rooms are introduced later, use this high-level architecture:

```text
Level Generator
       ↓
Room Database
       ↓
Room Prefab
       ↓
Room Connector
       ↓
Placement / Collision
       ↓
Generated Map
       ↓
NavMesh Build
       ↓
Enemy Spawn
```

The important rule is:

> Generation must finish before systems that depend on generated geometry are initialized.

For example:

```text
Room generation
    ↓
NavMesh baking
    ↓
Enemy spawning
```

not:

```text
Enemy spawning
    ↓
Room generation
    ↓
NavMesh baking
```

---

# 64. Development Checklist for New Features

Before declaring a feature complete:

### Code

- [ ] Script has one clear responsibility.
- [ ] References are assigned.
- [ ] Null cases handled.
- [ ] No unnecessary global searches.
- [ ] No duplicated logic.

### Input

- [ ] Input action exists.
- [ ] Binding exists.
- [ ] Action map enabled.
- [ ] Action is subscribed/unsubscribed correctly.

### Animation

- [ ] Animator parameter exists.
- [ ] Transition exists.
- [ ] Animation event exists if required.
- [ ] Collider timing is correct.

### Physics

- [ ] Collider type correct.
- [ ] Trigger state correct.
- [ ] Layer/tag correct.
- [ ] Rigidbody/CharacterController requirements satisfied.

### AI

- [ ] NavMesh exists.
- [ ] NavMeshAgent configured.
- [ ] Player tag correct.
- [ ] Attack range correct.
- [ ] Death stops AI.

### Data

- [ ] Data asset created.
- [ ] Database contains it.
- [ ] Runtime system references database.

### UI

- [ ] UI references assigned.
- [ ] UI updates when data changes.
- [ ] UI does not own gameplay state.

---

# 65. Important Current-Project Gaps

The README describes a broader RPG architecture than the actual uploaded code currently contains.

### Described by README but not present as current gameplay C# implementation

```text
Inventory
Equipment
ItemDatabase
Interaction interface
Interaction UI manager
SaveManager
Quest framework
Dialogue system
NPC AI
EnemySpawner
Boss-specific scripts
Patrol system
Skill system
Loot system
```

This distinction is important.

The documentation should not assume those systems already exist merely because they are mentioned in the README.

When implementing them, they should be added deliberately to the current architecture.

---

# 66. Recommended Next Architecture Step

Before adding many more RPG systems, establish a small `Core` layer:

```text
Core
├── GameManager
├── SceneManager
├── SaveManager
├── InputManager
└── EventBus
```

Then feature systems can remain independent:

```text
Player
Enemy
Combat
Inventory
Quest
Dialogue
Interaction
UI
```

This will prevent the project from turning into a chain of:

```text
GameObject.Find()
GetComponent()
FindObjectOfType()
```

dependencies.

---

# 67. Practical "Where Do I Change It?" Table

| I want to change... | Start here |
|---|---|
| Player HP | `Player.cs` |
| Player damage | `Player.cs` |
| Player movement speed | `PlayerMovement.cs` |
| Sprint | `PlayerMovement.cs` |
| Jump | `PlayerMovement.cs` |
| Gravity | `PlayerMovement.cs` |
| Ground detection | `PlayerMovement.cs` |
| Player states | `PlayerStateMachine.cs` |
| Player attack | `PlayerCombat.cs` |
| Weapon damage | `Player.cs` / `Weapon.cs` |
| Weapon hit behavior | `Weapon.cs` |
| Attack hit timing | `WeaponColliderBehaviourInAnimation.cs` / Animator |
| Enemy HP | `Enemy.cs` |
| Boss phases | `Enemy.cs` |
| Enemy chase | `EnemyAI.cs` |
| Enemy attack | `EnemyCombat.cs` |
| Enemy states | `EnemyStateMachine.cs` |
| Meteor | `Meteor.cs` |
| Enemy attack collision | `NormalAttackCollider.cs` |
| Camera look | `CameraLook.cs` |
| Input | `InputSystem_Actions.inputactions` |
| Player animation states | `PlayerStateMachine.cs` + Animator |
| Enemy animation states | `EnemyStateMachine.cs` + Animator |
| Navigation | `NavMeshAgent` + AI Navigation |
| Map geometry | `Assets/Map/` |
| Player model | `Assets/Character/MainCharacter/` |
| Enemy model | `Assets/Character/Enemy/` |
| Weapon model | `Assets/Character/Weapon/` |

---

# 68. Golden Rule for Extending the Project

When adding a feature, follow:

```text
DATA
 ↓
SYSTEM
 ↓
COMPONENT
 ↓
ANIMATION / PHYSICS / INPUT
 ↓
UI
```

Do not make UI own gameplay logic.

Do not make animation own permanent gameplay state.

Do not make the Player directly control every system.

Do not make EnemyAI control health.

Do not make EnemyCombat control navigation.

Keep each system responsible for its own domain.

---

# 69. Final Architecture Diagram

The current project can be understood as:

```text
                         INPUT SYSTEM
                              │
             ┌────────────────┴────────────────┐
             │                                 │
          PLAYER                             CAMERA
             │                                 │
      ┌──────┼────────┐                        │
      │      │        │                        │
   Movement Combat  State                  CameraLook
      │      │        │
      │      │        └─────────→ Animator
      │      │
      │      └────────→ Attack Trigger
      │                       │
      │                       ↓
      │                 Attack Animation
      │                       │
      │              Weapon Collider Behaviour
      │                       │
      │                       ↓
      │                    Weapon
      │                       │
      │                       ↓
      │                    Enemy
      │                       │
      ├───────────────────────┤
      │                       │
      ↓                       ↓
   Character              Enemy Health
  Controller                   │
                               ↓
                            Enemy AI
                               │
                    ┌──────────┴──────────┐
                    │                     │
                 NavMesh              Combat
                    │                     │
                    ↓              ┌──────┴──────┐
                 Movement         Normal       Meteor
```

This is the architecture actually represented by the current gameplay scripts.

---

# 70. Summary

The uploaded project is a **Unity 6 third-person Action RPG prototype** with a solid initial separation between:

- player movement
- player combat
- player state
- enemy health
- enemy AI
- enemy combat
- enemy state
- weapon hit detection
- animation-driven combat timing
- camera input

The most important current extension points are:

```text
InputSystem_Actions.inputactions
PlayerMovement
PlayerCombat
PlayerStateMachine
Player
Enemy
EnemyAI
EnemyCombat
EnemyStateMachine
Weapon
WeaponColliderBehaviourInAnimation
```

The README describes a much larger target architecture. As those systems are implemented, the safest approach is to add them as independent modules rather than expanding the existing Player/Enemy scripts into giant managers.



# Appendix A — Repository Inventory

Total files in repository: 8093

Approximate asset counts:

- `*.cs`: 18
- `*.unity`: 2
- `*.fbx`: 859
- `*.png`: 825
- `*.glb`: 749
- `*.obj`: 747
- `*.mtl`: 747
- `*.asset`: 32
- `*.controller`: 2
- `*.inputactions`: 1

Gameplay C# files outside `TutorialInfo`:

- `Assets/Scripts/CameraLook.cs`
- `Assets/Scripts/Enemy.cs`
- `Assets/Scripts/EnemyAI.cs`
- `Assets/Scripts/EnemyCombat.cs`
- `Assets/Scripts/EnemyStateMachine.cs`
- `Assets/Scripts/Gravity.cs`
- `Assets/Scripts/Meteor.cs`
- `Assets/Scripts/NormalAttackCollider.cs`
- `Assets/Scripts/Player.cs`
- `Assets/Scripts/PlayerCollusionDetector.cs`
- `Assets/Scripts/PlayerCombat.cs`
- `Assets/Scripts/PlayerMovement.cs`
- `Assets/Scripts/PlayerStateMachine.cs`
- `Assets/Scripts/Weapon.cs`
- `Assets/WeaponColliderBehaviourInAnimation.cs`
