using Godot;

[Tool, GlobalClass]
public partial class SnapToHexGrid : Node
{
	private Node2D _parent;

	public override void _Ready()
	{
		UpdateParentAndProcess();
	}

	public override void _Process(double delta)
	{
		if(_parent == null)
		{
			UpdateParentAndProcess();

			if(_parent == null)
			{
				return;
			}
		}

		Vector2 position = _parent.Position;
		position = AOECreator.CoordsToGlobalPosition(AOECreator.GlobalPositionToCoords(position));
		_parent.Position = position;

		float rotation = _parent.RotationDegrees;
		rotation = Mathf.RoundToInt(rotation / 60f) * 60f;
		_parent.RotationDegrees = rotation;
	}

	private void UpdateParentAndProcess()
	{
		if(!Engine.IsEditorHint())
		{
			SetProcess(false);
			return;
		}

		_parent = GetParentOrNull<Node2D>();

		if(_parent == null)
		{
			SetProcess(false);
			GD.Print($"Parent is not of type Node2D. Parent is {GetParent()?.Name}.");
		}
	}
}