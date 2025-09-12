using Godot;

public partial class FullScreenControl : Control
{
	public override void _Process(double delta)
	{
		UpdateToViewport();
	}

	private void UpdateToViewport()
	{
		// Set the global position and size so the control covers the screen
		Camera2D camera = GameController.Instance.CameraController.Camera;
		Size = GetViewport().GetVisibleRect().Size / camera.Zoom;
		GlobalPosition = (camera.GlobalPosition) - Size * 0.5f;
	}
}