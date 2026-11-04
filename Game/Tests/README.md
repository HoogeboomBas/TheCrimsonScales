# Wish Upon a Star regression

`WishUponAStarTests` runs inside Godot 4.7.1 .NET using gdUnit4Net. It creates a real gameplay scene in a 10×10 room and plays Wish Upon a Star through the real card engine. Parameterized cases vary both allies' starting damage and cover Dark consumed, available but declined, and unavailable. Only healed targets at full health may receive Bless, and only when Dark is consumed. Separate cases cover targeting the Starslinger and an already-full-health ally.

Set `GODOT_BIN` to your Godot .NET executable, including in the environment inherited by Rider:

```bash
export GODOT_BIN=/path/to/Godot_v4.7.1-stable_mono_linux.x86_64
dotnet test Tests/Game.Tests.csproj
```

The project uses `test.runsettings` for headless execution, sequential tests, and runtime roll-forward. Rider uses VSTest discovery after building the solution; new tests may require a build through Rider before their run icons appear.

For an optional visible run:

```bash
dotnet test Tests/Game.Tests.csproj -- GdUnit4.Parameters="--windowed --rendering-method gl_compatibility --audio-driver Dummy" RunConfiguration.EnvironmentVariables.CRIMSON_TEST_SHOW_GAME=1
```

The fixture supports only character placement, starting cards, and scripted prompt answers needed by this regression. It resolves the real card side, including effects and final card state, without starting the automatic round loop. Unexpected prompts and unused answers fail the test. Sessions must be disposed with `await using`.

Production hooks allow configuring the gameplay scene before it enters the tree, selecting the fixture room, pausing automatic scenario phases, and injecting decisions into `PromptManager`. Target choices use the game's valid-target check. Normal gameplay defaults remain interactive.

The separate Godot project isolates test packages, import caches, saves, and logs. Its build generates the runnable Godot project and production resource copies under `Tests/obj/godot/` to preserve existing `res://` paths, while referencing the compiled game assembly. The authored `Tests/project.godot` is the configuration template. `TestBootstrap` registers that assembly's Godot scripts before autoloads start. After deleting or renaming production resources, run `dotnet clean Tests/Game.Tests.csproj` before rebuilding to remove stale copies.

After building, open `Tests/obj/godot/project.godot` to inspect `Fixtures/TestRoom.tscn`. Edit production assets in the main project and rebuild to refresh the copies. The adapter runner and Godot import cache also stay under the ignored generated project.
