using System;
using Godot;

public partial class AOEButtonView : Control
{
	[Export]
	private Control _buttonContainer;
	[Export]
	private ChoiceButton _mirrorButton;
	[Export]
	private ChoiceButton _rotateCounterClockwiseButton;
	[Export]
	private ChoiceButton _rotateClockwiseButton;

	public event Action MirrorPressed;
	public event Action RotateCounterClockwisePressed;
	public event Action RotateClockwisePressed;

	public override void _Ready()
	{
		base._Ready();

		Show();

		_mirrorButton.BetterButton.Pressed += OnMirrorPressed;
		_rotateCounterClockwiseButton.BetterButton.Pressed += OnRotateCounterClockwisePressed;
		_rotateClockwiseButton.BetterButton.Pressed += OnRotateClockwisePressed;
	}

	public void Open(bool mirrorEnabled)
	{
		_mirrorButton.SetActive(mirrorEnabled);
		_rotateCounterClockwiseButton.SetActive(true);
		_rotateClockwiseButton.SetActive(true);

		_mirrorButton.SetPosition(new Vector2(_buttonContainer.Position.X, GameController.Instance.EffectSelectionView.EffectCount > 0 ? -150f : 0f));
	}

	public void Close()
	{
		_mirrorButton.SetActive(false);
		_rotateCounterClockwiseButton.SetActive(false);
		_rotateClockwiseButton.SetActive(false);
	}

	private void OnMirrorPressed()
	{
		MirrorPressed?.Invoke();
	}

	private void OnRotateCounterClockwisePressed()
	{
		RotateCounterClockwisePressed?.Invoke();
	}

	private void OnRotateClockwisePressed()
	{
		RotateClockwisePressed?.Invoke();
	}
}