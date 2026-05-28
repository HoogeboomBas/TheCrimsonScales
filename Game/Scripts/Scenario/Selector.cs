using System;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class Selector : Node2D
{
	[Export]
	private Node2D _scaleContainer;

	private Vector2I _currentCoords;

	public event Action<Vector2I, bool> CoordsChangedEvent;

	public override void _Ready()
	{
		base._Ready();

		const float startScale = 1f;
		_scaleContainer.Scale = startScale * Vector2.One;
		GTween tween = GTweenSequenceBuilder.New()
			.Append(_scaleContainer.TweenScale(0.95f * Vector2.One, 1f).SetEasing(Easing.OutQuad))
			.AppendTime(0.2f)
			.Append(_scaleContainer.TweenScale(startScale * Vector2.One, 1f).SetEasing(Easing.InOutQuad))
			.AppendTime(0.2f)
			.Build();

		tween.SetMaxLoops();
		tween.Play();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		Vector2 globalMousePosition = GetGlobalMousePosition();
		Vector2I oldCoords = _currentCoords;
		bool oldVisible = Visible;
		_currentCoords = Map.GlobalPositionToCoords(globalMousePosition);

		Hex hex = GameController.Instance.Map.GetHex(_currentCoords);
		SetVisible(!GameController.Instance.CursorOverUIChecker.CursorOverUI && (hex?.Revealed ?? false));

		if(oldCoords == _currentCoords && oldVisible == Visible)
		{
			return;
		}

		GlobalPosition = hex?.GlobalPosition ?? Vector2.Zero;
		CoordsChangedEvent?.Invoke(_currentCoords, Visible);
		//GD.Print(hex?.Coords);
	}
}