using Godot;

public partial class FigureViewComponent : HexObjectViewComponent
{
	[Export]
	public Node2D Health { get; private set; }

	[Export]
	public TextureProgressBar HealthProgressBar { get; private set; }

	[Export]
	public Curve HealthProgressBarCurve { get; private set; }

	[Export]
	public Label HealthLabel { get; private set; }

	[Export]
	public Node2D Shield { get; private set; }

	[Export]
	public Sprite2D ShieldIcon { get; private set; }

	[Export]
	public Sprite2D CrackedShieldIcon { get; private set; }

	[Export]
	public Label ShieldLabel { get; private set; }

	[Export]
	public Node2D Retaliate { get; private set; }

	[Export]
	public Label RetaliateLabel { get; private set; }

	[Export]
	public Node2D Flying { get; private set; }

	[Export]
	public Node2D EffectParent { get; private set; }

	[Export]
	public GpuParticles2D TurnStartPS { get; private set; }

	[Export]
	public Node2D ActivePS { get; set; }

	public override void OnHexesChanged(HexObject hexObject)
	{
		base.OnHexesChanged(hexObject);

		if(HexObject.GetParentOfType<Figure>() != null)
		{
			// This figure is following another figure, it does not need to adjust the view position
			return;
		}

		Vector2 bestHexGlobalPosition = HexObject.GlobalPosition + 10000f * Vector2.Up + 10000f * Vector2.Left;

		// Find bottom-left-most position
		foreach(Hex hex in HexObject.Hexes)
		{
			if(hex != null)
			{
				float diff = hex.GlobalPosition.Y - bestHexGlobalPosition.Y;
				if(Mathf.Abs(diff) < 0.1f && hex.GlobalPosition.X < bestHexGlobalPosition.X)
				{
					bestHexGlobalPosition = hex.GlobalPosition;
				}
				else if(diff > 0.1f)
				{
					bestHexGlobalPosition = hex.GlobalPosition;
				}
			}
		}

		SetGlobalPosition(bestHexGlobalPosition);
		SetGlobalRotation(0f);
	}
}