using Godot;
using Godot.Bridge;

namespace CrimsonScales.Tests;

/// <summary>Register scripts from the referenced game assembly before Godot loads its autoloads and scenes.</summary>
public partial class TestBootstrap : Node
{
	public TestBootstrap()
	{
		ScriptManagerBridge.LookupScriptsInAssembly(typeof(GameController).Assembly);
	}
}
