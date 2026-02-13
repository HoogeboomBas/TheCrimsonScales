using Godot;
using GTweensGodot.Extensions;

public partial class ScenarioWonView : Control
{
	[Export]
	private ChoiceButton _continueButton;

	public override void _Ready()
	{
		base._Ready();

		_continueButton.SetActive(true);

		_continueButton.BetterButton.Pressed += OnContinuePressed;

		Hide();
	}

	public void Open()
	{
		Show();

		this.TweenModulateAlpha(0f, 0f).Play(true);
		this.TweenModulateAlpha(1f, 0.5f).Play();
	}

	private void OnContinuePressed()
	{
		GameController.Instance.EndScenario(ScenarioResult.Win);
	}
}