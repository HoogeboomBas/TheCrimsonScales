# Godot card scenarios

The separate `Game.Tests.csproj` references `../Game.csproj` and runs C# integration tests in the **Godot 4.7.1 .NET engine**, using gdUnit4Net. Testing packages, discovery, fixtures, and scenario builders belong only to the test project. The normal game entry scene is not started. Each test creates the real gameplay scene and a fresh 10×10 hex room, supplies its initial state, and resolves cards through `AbilityCardSide.Perform`.

## Run

Set `GODOT_BIN` to your Godot **.NET** executable (also in the environment inherited by Rider):

```bash
export GODOT_BIN=/path/to/Godot_v4.7.1-stable_mono_linux.x86_64
dotnet test Tests/Game.Tests.csproj
dotnet test Tests/Game.Tests.csproj --filter 'FullyQualifiedName~Starting_poison'
```

The project selects `Tests/test.runsettings` by default. It configures headless execution, sequential runs, timeouts, and allows an installed newer .NET runtime to host the .NET 8 tests. In Rider, select that same runsettings file if it does not pick up the project default. You can also pass `--settings Tests/test.runsettings` explicitly.

For a visible run, override that configuration on the command line:

```bash
dotnet test Tests/Game.Tests.csproj --filter 'FullyQualifiedName~Basic_attack' -- GdUnit4.Parameters="--windowed --rendering-method gl_compatibility --audio-driver Dummy" RunConfiguration.EnvironmentVariables.CRIMSON_TEST_SHOW_GAME=1
```

This opens the same test in a window, enables gameplay animations, and briefly pauses before and after card execution. There is no persistent preview. The main game project is not a test project.

## Write a scenario

```csharp
using System.Threading.Tasks;
using CrimsonScales.Tests;
using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

[TestSuite, RequireGodotRuntime]
public class MyCardTests
{
    [TestCase(Timeout = 30000), GodotExceptionMonitor]
    public async Task Poison_increases_basic_attack_damage()
    {
        await using TestGame game = await TestGame.Start(
            ScenarioSetup.InTestRoom()
                .WithCharacter<BombardModel>(out var hero, new Vector2I(3, 4))
                .WithCard<DoubleCannons>(hero)
                .WithMonster<BanditGuard>(out var guard, new Vector2I(4, 4), health: 10)
                .WithCondition(guard, Conditions.Poison1)
                .WithNeutralAttackModifiers());

        await game.PlayCard<DoubleCannons>(hero, AbilityCardSection.BasicTop,
            choices => choices.SelectTarget(guard));

        AssertThat(guard.Unit.Health).IsEqual(7);
    }
}
```

- `WithCharacter<T>(out var hero, at)` and `WithMonster<T>(out var guard, at)` return typed handles. Reuse them in `WithCard`, `WithCondition`, `PlayCard`, and `SelectTarget`; access the spawned unit through `hero.Unit` or `guard.Unit`. Handles resolve only during an active session and can be reused when restarting the same setup. Named declarations remain available when a label is useful.
- Coordinates are axial `(q,r)`, each ranging from 0 through 9. Open `Fixtures/TestRoom.tscn` in Godot to inspect the labeled board.
- `WithCard<T>` chooses the character's starting hand. Without it, the class's default hand is used. Use cards belonging to the selected class.
- `WithCondition` applies an existing buff/debuff through the real condition lifecycle, including subscriptions. Supply `giver: hero` for conditions requiring a source.
- `WithOverlay("res://Content/OverlayTiles/Obstacles/Bush1H.tscn", at)` instantiates a real obstacle or terrain scene and registers its footprint. Doors and custom room topology are not implemented in this initial single-room fixture.
- `WithSeed` sets the gameplay RNG (default 42). `WithNeutralAttackModifiers` replaces character modifier decks with +0 cards, including draws under advantage/disadvantage; omit it when testing actual decks.
- `PlayCard<T>` supports printed top/bottom and basic top/bottom, requires the card in hand, and awaits resolution, including resulting card state.
- Choices are supplied in order. `SelectTarget`, `MoveTo`, and `Confirm` cover the initial cases. `MoveTo` uses production pathfinding. If movement points remain, supply another movement answer or an explicit skipped `MovePrompt.Answer`.
- `Answer<TPrompt>(prompt => answer)` supports other prompt types. This low-level escape hatch must supply a legal answer; prefer validated helpers for targets and movement.
- Unexpected prompts and unused scripted answers fail the test. Card resolution has a 15-second timeout; use a test timeout as well to bound initialization.

## Scope and isolation

This initial harness tests **card-half resolution**, not complete turns: initiative, two-card selection, turn-start/end effects, monster turns, and round transitions are not driven automatically. There is no direct attack/move command API. Starting conditions are setup; gameplay effects are executed through cards.

`PromptManager` receives an `IPromptDecisionSource` through its constructor. Normal game requests default to `InteractiveDecisionSource`; the test fixture injects its `ScriptedChoices` instance before initialization. Recorded answers, effects, and undo remain in the manager. Scripted choices are cleared after each card, and unexpected prompts fail immediately.

Always dispose the session (`await using`). Gameplay sessions are sequential because the game uses singletons. The included suite verifies that a second session starts without previous health, condition, or card state.

The test project has its own `project.godot`, assembly (`Game.Tests`), import cache, and Godot user-data directory (`TheCrimsonScales.Tests`). Saves and application logs from autoloads therefore stay separate from developer campaigns without test-specific changes to `SaveManager` or `Log`. TestGame refuses to run under the ordinary game configuration.

`Game.csproj` excludes the entire `Tests/` tree from compilation and has no test-framework dependencies. Export configurations in `Game.sln` do not build the test project.

To preserve the game's existing `res://` paths, the test build incrementally copies game resources into ignored directories inside `Tests/`. Scripts in those copies are resources only: they are not recompiled into the test assembly. The test project uses the game assembly through its project reference; a test-only bootstrap registers that assembly's scripts with Godot before the production autoloads load. The initial copy and Godot asset import take longer than subsequent runs. After deleting or renaming production resources, use `dotnet clean Tests/Game.Tests.csproj` before rebuilding to remove old copies.

The adapter generates `Tests/gdunit4_testadapter_v5/` on first execution; it is ignored by Git and compiled only by the test project. Open `Tests/project.godot` to inspect the test room and resource copies in Godot. Edit production assets in the main game project, then rebuild the tests to refresh the copies.
