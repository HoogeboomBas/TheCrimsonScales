﻿using System;
using Godot;
using GTweens.Easings;

public partial class AOEHexView : Node2D
{
	[Export]
	private WorldButton _worldButton;

	public event Action<AOEHexView, Vector2I> DraggedEvent;
	public event Action<AOEHexView> PressedEvent;

	public AOEHexType Type { get; private set; }

	public bool Pressed { get; private set; }
	public bool Dragging { get; private set; }
	public Vector2I GlobalCoords { get; private set; }

	public void Init(AOEHex hex)
	{
		Type = hex.Type;

		Position = Map.CoordsToGlobalPosition(hex.LocalCoords);

		Scale = Vector2.Zero;
		this.TweenScale(1f, 0.15f).SetEasing(Easing.OutBack).PlayFastForwardable();

		_worldButton.PressedEvent += OnPressed;
		_worldButton.DraggedEvent += OnDragged;
	}

	public void Destroy()
	{
		this.TweenScale(0f, 0.15f).SetEasing(Easing.InBack).PlayFastForwardable();
	}

	public void SetCoords(Vector2I coords)
	{
		GlobalCoords = coords;
	}

	private void OnPressed()
	{
		PressedEvent?.Invoke(this);
	}

	private void OnDragged(Vector2 previousPosition, Vector2 currentPosition)
	{
		Vector2 globalMousePosition = GetGlobalMousePosition();
		Vector2I coords = Map.GlobalPositionToCoords(globalMousePosition);

		Vector2I delta = coords - GlobalCoords;

		if(delta != Vector2I.Zero)
		{
			DraggedEvent?.Invoke(this, delta);
		}
	}
}