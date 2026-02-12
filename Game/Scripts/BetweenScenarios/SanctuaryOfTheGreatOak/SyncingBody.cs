using Godot;

public partial class SyncingBody : AnimatableBody3D
{
	[Export]
	private Node3D _nodeToFollow;

	public override void _Process(double delta)
	{
		base._Process(delta);

		SetGlobalTransform(_nodeToFollow.GlobalTransform);
	}
}