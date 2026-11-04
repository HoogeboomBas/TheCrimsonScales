# Wish Upon a Star regression

`WishUponAStarTests` runs inside Godot 4.7.1 .NET using gdUnit4Net. It creates a real gameplay scene in a 10×10 room, gives Wish Upon a Star to a Starslinger, and places two injured allies nearby. Playing the card's top with Dark must heal both allies to full and add one Bless to each modifier deck.

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

The separate Godot project isolates test packages, import caches, saves, and logs. Its build copies production resources into ignored directories to preserve existing `res://` paths, while referencing the compiled game assembly. `TestBootstrap` registers that assembly's Godot scripts before autoloads start. After deleting or renaming production resources, run `dotnet clean Tests/Game.Tests.csproj` before rebuilding to remove stale copies.

Open `Tests/project.godot` to inspect `Fixtures/TestRoom.tscn`. Edit production assets in the main project and rebuild to refresh the copies. The generated `gdunit4_testadapter_v5` directory is ignored by Git.
