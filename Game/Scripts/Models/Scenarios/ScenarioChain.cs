using Godot;

public abstract class ScenarioChain : AbstractModel
{
	public virtual ScenarioChain BaseScenarioChain => this;

	public abstract Color Color { get; }
}