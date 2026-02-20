using System;
using Godot;
using GTweensGodot.Extensions;

public partial class BetweenScenariosClassUnlockOverlay : Control
{
	[Export]
	private UnlockCharacterView _unlockCharacterView;

	public override void _Ready()
	{
		base._Ready();

		_unlockCharacterView.ClosedEvent += OnViewClosed;
		_unlockCharacterView.SkipButtonPressedEvent += OnSkipPressed;
		Hide();
	}

	public void Open(ClassModel classModel)
	{
		Show();
		SetModulate(Colors.Transparent);
		this.TweenModulateAlpha(1f, 0.3f).Play();

		_unlockCharacterView.Open(classModel, BetweenScenariosController.Instance.DestroyCancellationToken);
	}

	private void Close()
	{
		this.TweenModulateAlpha(0f, 0.3f).OnComplete(Hide).Play();
	}

	private void OnViewClosed()
	{
		Close();
	}

	private void OnSkipPressed()
	{
		_unlockCharacterView.Skip();
	}
}