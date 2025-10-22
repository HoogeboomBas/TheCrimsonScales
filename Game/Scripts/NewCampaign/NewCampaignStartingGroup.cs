using System;
using Godot;

public partial class NewCampaignStartingGroup : BetterButton
{
	[Export]
	private BetterButton _button;

	[Export]
	public StartingGroup StartingGroup { get; private set; }

	public event Action<NewCampaignStartingGroup> PressedEvent;

	public override void _Ready()
	{
		base._Ready();

		_button.Pressed += OnPressed;
	}

	private void OnPressed()
	{
		PressedEvent?.Invoke(this);
	}
}