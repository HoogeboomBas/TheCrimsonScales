using Godot;

public sealed partial class OverlayTileViewComponent : HexObjectViewComponent
{
	[Export]
	private Node2D _icon;

	public override void OnHexesChanged(HexObject hexObject)
	{
		base.OnHexesChanged(hexObject);

		Vector2 bestHexPosition = HexObject.GlobalPosition + 10000f * Vector2.Up + 10000f * Vector2.Left;

		// Find bottom-left-most position
		foreach(Hex hex in HexObject.Hexes)
		{
			if(hex != null)
			{
				float diff = hex.GlobalPosition.Y - bestHexPosition.Y;
				if(Mathf.Abs(diff) < 0.1f && hex.GlobalPosition.X < bestHexPosition.X)
				{
					bestHexPosition = hex.GlobalPosition;
				}
				else if(diff > 0.1f)
				{
					bestHexPosition = hex.GlobalPosition;
				}
			}
		}

		_icon.SetGlobalPosition(bestHexPosition + 90f * Vector2.Down);
		_icon.SetGlobalRotation(0f);
	}
}