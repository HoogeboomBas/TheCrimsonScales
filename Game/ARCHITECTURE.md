# Codebase architecture guide

This repository contains the Godot implementation of The Crimson Scales and a companion set of Godot-based content tools. The main game is written in C# and targets Godot 4.6.1 .NET; see [Game.csproj](./Game.csproj) and [project.godot](./project.godot) for project and engine configuration.

## Repository map

- [`Game/`](.) is the playable Godot game project.
- [`TextureCreator/`](../TextureCreator/) is a separate Godot project containing small authoring tools, including tools for areas of effect, cards, markers, and overlay tiles.
- [`README.md`](../README.md) describes the game, installation, contribution, and licensing context.

### Main game project

- [`Scripts/`](./Scripts/) contains shared runtime systems, organized by responsibility:
  - `MainMenu`, `NewCampaign`, `BetweenScenarios`, and `UnlockCharacter` implement campaign and downtime interfaces.
  - `SceneLoading` coordinates scene requests and transitions.
  - `SavedData` defines persisted campaign/device state and save-file handling.
  - `Models` contains reusable game definitions and model lookup.
  - `Scenario` contains encounter runtime state, turn and phase management, map/figure logic, prompts, events, and combat UI.
  - `UI`, `Utils`, and `Audio` provide shared presentation and support code.
- [`Content/`](./Content/) contains game-specific definitions and assets. C# definitions are grouped by content family: `Classes`, `Monsters`, `Items`, `Scenarios`, `Events`, `BattleGoals`, `PartyGoals`, `PersonalQuests`, and other game content. Many definitions have matching Godot scenes, images, or imported resources nearby.
- [`Scenes/`](./Scenes/) contains Godot scene trees for the main menu, campaign flow, and scenario UI. Scene controllers use exported node references to connect C# behavior to those trees.
- [`Art/`](./Art/), [`Fonts/`](./Fonts/), [`Materials/`](./Materials/), [`Resources/`](./Resources/), and [`Shaders/`](./Shaders/) hold presentation resources.
- [`addons/`](./addons/) and [`GTweensGodot/`](./GTweensGodot/) contain engine integrations and supporting libraries. Check their local licenses and documentation when working in those directories.

## Runtime flow

### Startup and shared services

Godot starts the main scene configured by `run/main_scene` in `project.godot`. The same file registers autoloads for the application controller, the GDTask player-loop integration, and the tween context. [`AppController`](./Scripts/AppController.cs) is the application-wide singleton: it creates the `SaveManager` and provides access to shared scene, popup, input, audio, and save services.

### Scene transitions

Screens are entered through typed `SceneRequest` objects rather than by passing loose arguments between scenes. [`SceneLoader`](./Scripts/SceneLoading/SceneLoader.cs) prevents overlapping transitions and transitions while saving is blocked, closes popups, displays a loading scene, loads the requested scene, waits for its controller's additional loading, and then completes the request. [`SceneController<T>`](./Scripts/SceneController.cs) supplies the common scene-singleton base and the additional-loading hook.

The main menu opens or creates a campaign. Campaign preparation and downtime take place in `BetweenScenarios`; that screen handles campaign actions such as character changes, events, rewards, and scenario selection. Selecting an encounter requests the gameplay scene with the campaign state.

### Scenario gameplay

[`GameController`](./Scripts/Scenario/GameController.cs) is the gameplay composition root. It receives or constructs the campaign request, resolves the selected `ScenarioModel`, instantiates that model's Godot scene, and creates the managers needed for the encounter (including characters, cards, elements, prompts, scenario events, and undo).

[`ScenarioPhaseManager`](./Scripts/Scenario/Phases/ScenarioPhaseManager.cs) runs scenario initialization and setup, then repeatedly runs card selection and the round phase. Scenario-specific rules and behavior live with the scenario definition: [`ScenarioModel`](./Scripts/Models/Scenarios/ScenarioModel.cs) provides the map scene path, story, monsters, rewards, goals, rules, and lifecycle hooks, while concrete definitions in `Content/Scenarios/` customize those hooks.

`ScenarioEvents` and `ScenarioCheckEvents` provide encounter-scoped extension points. Content such as character perks, items, monsters, and scenarios subscribes to relevant events or checks to add behavior without placing every special rule in the turn loop. Subscriptions should be scoped and removed according to the owning system's lifecycle.

## Content and model conventions

Most reusable game definitions are model classes, with concrete definitions under `Content/`. The shared model types in `Scripts/Models/` define the contracts and common behavior. For example, a character class model supplies its card and perk lists, statistics, colors, and scene; a scenario model supplies its map scene, monsters, story, and rewards.

[`ModelDB`](./Scripts/Models/ModelDB/ModelDB.cs) is the central typed lookup and cache for models. It derives stable IDs from model types and can discover concrete subclasses when resolving a saved ID. Definitions commonly request other definitions through typed helpers such as `ModelDB.Class<T>()`, `ModelDB.Item<T>()`, or `ModelDB.Scenario<T>()`.

Keep the distinction between definitions and play state in mind: model objects describe reusable content, while `Saved*` objects represent campaign/device persistence and mutable progress. Encounter-local state is owned by runtime objects and managers under `Scripts/Scenario/`; it should not be treated as permanent campaign state unless it is explicitly copied into saved data.

When adding content, start from a nearby sibling in the same `Content/` category and follow its matching model, scene, asset, and ID conventions. Scenario and class definitions often reference other content through `ModelDB`, so update the relevant links, unlocks, or content lists as well as adding the definition itself.

## Persistence

[`SaveManager`](./Scripts/SavedData/SaveManager.cs) manages one device save and three campaign save slots. [`SaveFile<TSaveData>`](./Scripts/SavedData/SaveFile.cs) serializes save data as JSON under Godot's `user://` directory and runs the save migration logic when loading existing files. Save operations can be temporarily blocked by active gameplay operations; scene changes respect that blocker.

## Companion authoring tools

`TextureCreator/` is independent of the playable project and has its own Godot project and .NET project file. Its subprojects focus on specific asset-authoring tasks (AOE, card position, marker, and overlay-tile work). Use the relevant tool project for its own asset workflow; runtime game behavior remains in `Game/`.
