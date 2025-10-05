using Godot;

public partial class PartyButton : Control
{
	[Export]
	private BetterButton _button;

	public override void _Ready()
	{
		base._Ready();

		_button.Pressed += OnPressed;
	}

	private void OnPressed()
	{
		AppController.Instance.PopupManager.RequestPopup(new PartyInfoPopup.Request
		{
			Characters = GameController.Instance.CharacterManager.Characters
		});
	}
}