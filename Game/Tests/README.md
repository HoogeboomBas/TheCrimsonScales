# Wish Upon a Star regression

`WishUponAStar_Top_Tests` runs inside Godot 4.7.1 .NET using gdUnit4Net. It creates a real gameplay scene in a 10×10 room and plays Wish Upon a Star through the real card engine. Parameterized cases vary both allies' starting damage and cover Dark consumed, available but declined, and unavailable. Only healed targets at full health may receive Bless, and only when Dark is consumed. Separate cases cover targeting the Starslinger and an already-full-health ally.

Set `GODOT_BIN` to your Godot .NET executable, including in the environment inherited by Rider:

```bash
export GODOT_BIN=/path/to/Godot_v4.7.1-stable_mono_linux.x86_64
dotnet test Tests/Game.Tests.csproj
```

The project uses `test.runsettings` for headless execution, sequential tests, and runtime roll-forward. Rider uses VSTest discovery after building the solution; new tests may require a build through Rider before their run icons appear.

For an optional visible run:

```bash
dotnet test Tests/Game.Tests.csproj -- \
  GdUnit4.Parameters="--windowed --rendering-method gl_compatibility --audio-driver Dummy" \
  RunConfiguration.EnvironmentVariables.CRIMSON_TEST_SHOW_GAME=1
```

In Rider, select the shared **Visual Test Run** configuration from the run dropdown. In **Run → Edit Configurations**, replace its `GODOT_BIN` placeholder with your Godot .NET executable path. This shell configuration has its own environment; it does not inherit Rider's unit-test runner environment settings. Keep machine-specific paths out of commits.

To run one test visually, duplicate **Visual Test Run**, give it a descriptive name, and add `--filter` before the standalone `--` in its **Script text**:

```bash
dotnet test Tests/Game.Tests.csproj \
  --filter "FullyQualifiedName~Starslinger_can_heal_and_bless_themselves" \
  -- \
  GdUnit4.Parameters="--windowed --rendering-method gl_compatibility --audio-driver Dummy" \
  RunConfiguration.EnvironmentVariables.CRIMSON_TEST_SHOW_GAME=1
```

To run every test in a class visually, filter by the class name instead:

```bash
dotnet test Tests/Game.Tests.csproj \
  --filter "FullyQualifiedName~WishUponAStar_Top_Tests" \
  -- \
  GdUnit4.Parameters="--windowed --rendering-method gl_compatibility --audio-driver Dummy" \
  RunConfiguration.EnvironmentVariables.CRIMSON_TEST_SHOW_GAME=1
```

VSTest filters by class or method name, not source filename. For a file containing one test class, filtering by that class runs all tests in the file.

Replace the method name to select another test. A parameterized method runs all its parameter cases. The configuration uses the saved filter; it does not select the test under the editor cursor. Results appear in the configuration's console. Normal gutter test runs continue to use the headless `test.runsettings` profile.

The fixture supports only character placement, starting cards, and scripted prompt answers needed by this regression. It scripts the production card-side selection prompt and calls `AbilityCmd.PerformAbilityCardTopOrBottom`, including effects and final card state, without starting the automatic round loop. Unexpected prompts and unused answers fail the test. Sessions must be disposed with `await using`.

`GameController.Configure` accepts the scene request and initialization options before the scene enters the tree. The options select the fixture room, pause automatic scenario phases, and inject decisions into `PromptManager`; `GameSceneRequest` contains no test initialization options. Target eligibility, selection state, and automatic confirmation/skip rules live in `TargetSelection`. `TargetSelectionPrompt` presents that state through the gameplay UI; scripted choices use the same selection object without opening UI. Normal gameplay defaults remain interactive.

The separate Godot project isolates test packages, import caches, saves, and logs. Its build generates the runnable Godot project and production resource copies under `Tests/obj/godot/` to preserve existing `res://` paths, while referencing the compiled game assembly. The authored `Tests/project.godot` is the configuration template. `TestBootstrap` registers that assembly's Godot scripts before autoloads start. After deleting or renaming production resources, run `dotnet clean Tests/Game.Tests.csproj` before rebuilding to remove stale copies.

After building, open `Tests/obj/godot/project.godot` to inspect `Fixtures/TestRoom.tscn`. Edit production assets in the main project and rebuild to refresh the copies. The adapter runner and Godot import cache also stay under the ignored generated project.
