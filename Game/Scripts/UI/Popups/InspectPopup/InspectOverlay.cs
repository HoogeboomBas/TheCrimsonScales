using System;
using Godot;

public partial class InspectOverlay : Control
{
	public event Action InspectRequestedEvent;

	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventMouseButton mouseEvent &&
		   mouseEvent.ButtonIndex == MouseButton.Right &&
		   mouseEvent.Pressed)
		{
			Vector2 mousePosition = GetGlobalMousePosition();

			bool clickedButton = GetGlobalRect().HasPoint(mousePosition);

			if(clickedButton)
			{
				InspectRequestedEvent?.Invoke();
			}
		}
	}
}
