using Godot;

public static class AOEExtensionMethods
{
	public static readonly Vector2I[] NeighbourOffsets =
	{
		new Vector2I(1, -1), // NorthEast
		new Vector2I(1, 0), // East
		new Vector2I(0, 1), // SouthEast
		new Vector2I(-1, 1), // SouthWest
		new Vector2I(-1, 0), // West
		new Vector2I(0, -1), //NorthWest
	};

	public static Vector2I Add(this Vector2I coords, Direction direction)
	{
		return coords + NeighbourOffsets[(int)direction];
	}
}