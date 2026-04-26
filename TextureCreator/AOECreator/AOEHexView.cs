using Godot;

public partial class AOEHexView : Node2D
{
	public void Init(AOEHex hex)
	{
		Position = AOECreator.CoordsToGlobalPosition(hex.Coords);
	}
}