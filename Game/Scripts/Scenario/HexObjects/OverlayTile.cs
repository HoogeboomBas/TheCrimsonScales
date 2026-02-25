using Godot;

public abstract partial class OverlayTile : HexObject
{
	[Export]
	private SFX.StepType _stepType;
	public override SFX.StepType? OverrideStepType => _stepType;
}