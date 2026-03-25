using System;
using System.Collections.Generic;
using Godot;

public partial class ChoiceButtonsView : Control
{
	[Export]
	private ChoiceButton _continueButton;
	[Export]
	private ChoiceButton _skipButton;

	private readonly List<object> _blockers = new List<object>();

	private bool _continueActive;
	private bool _skipActive;

	private event Action ContinuePressedEvent;
	private event Action SkipPressedEvent;

	public override void _Ready()
	{
		base._Ready();

		Show();

		_continueButton.BetterButton.Pressed += OnContinuePressed;
		_skipButton.BetterButton.Pressed += OnSkipPressed;
	}

	public void Open(Action onContinuePressed, Action onSkipPressed)
	{
		ContinuePressedEvent = onContinuePressed;
		SkipPressedEvent = onSkipPressed;

		// UpdateButtons();
	}

	public void SetButtons(bool continueActive, bool skipActive)
	{
		_continueActive = continueActive;
		_skipActive = skipActive;

		UpdateButtons();
	}

	public void Close()
	{
		_continueActive = false;
		_skipActive = false;

		UpdateButtons();
	}

	public void Block(object blocker)
	{
		_blockers.AddIfNew(blocker);

		UpdateButtons();
	}

	public void UnBlock(object blocker)
	{
		_blockers.Remove(blocker);

		UpdateButtons();
	}

	private void UpdateButtons()
	{
		_continueButton.SetActive(ContinuePressedEvent != null && _continueActive && _blockers.Count == 0);
		_skipButton.SetActive(SkipPressedEvent != null && _skipActive && _blockers.Count == 0);
	}

	private void OnContinuePressed()
	{
		ContinuePressedEvent?.Invoke();
	}

	private void OnSkipPressed()
	{
		SkipPressedEvent?.Invoke();
	}
}