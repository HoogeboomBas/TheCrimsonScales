﻿using Godot;
using GTweensGodot.Extensions;

public partial class ScenarioLostView : Control
{
	[Export]
	private ChoiceButton _retryButton;
	[Export]
	private ChoiceButton _returnToTownButton;

	public override void _Ready()
	{
		base._Ready();

		_retryButton.SetActive(true);
		_returnToTownButton.SetActive(true);

		_retryButton.BetterButton.Pressed += OnRetryPressed;
		_returnToTownButton.BetterButton.Pressed += OnReturnToTownPressed;

		Hide();
	}

	public void Open()
	{
		Show();

		this.TweenModulateAlpha(0f, 0f).Play(true);
		this.TweenModulateAlpha(1f, 0.5f).Play();
	}

	private void OnRetryPressed()
	{
		GameController.Instance.EndScenario(false, false);
	}

	private void OnReturnToTownPressed()
	{
		GameController.Instance.EndScenario(true, false);
	}
}