using Godot;

public class AOEHex
{
	public Vector2I Coords { get; }
	public AOEHexType Type { get; }
	public string CustomMark { get; }
	public string IconPath { get; }

	public AOEHex(Vector2I coords, AOEHexType type, string customMark = null, string iconPath = null)
	{
		Coords = coords;
		Type = type;
		CustomMark = customMark;
		IconPath = iconPath;
	}
}