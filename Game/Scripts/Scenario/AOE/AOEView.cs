﻿using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class AOEView : Node2D
{
	[Export]
	private PackedScene _redHexScene;
	[Export]
	private PackedScene _grayHexScene;
	[Export]
	private PackedScene _emptyHexScene;

	[Export]
	private Node2D _hexParent;

	private Figure _performer;
	private int _range;
	private Hex _forcedOriginHex;
	private bool _hasGrayHex;

	private Vector2I _coords;
	private int _rotationIndex;

	private readonly HashSet<Vector2I> _possibleHexes = new HashSet<Vector2I>();
	private readonly List<Vector2I> _coordsCache = new List<Vector2I>();

	private GTween _moveTween;

	public List<AOEHexView> Hexes { get; } = new List<AOEHexView>();

	public event Action AOEChangedEvent;

	public void Open(AOEPattern pattern, Hex forcedOriginHex, Figure performer, int range)
	{
		Close();

		_performer = performer;
		_range = range;
		_forcedOriginHex = forcedOriginHex;
		_hasGrayHex = false;

		_coords = _performer.Hex.Coords;
		_rotationIndex = 0;

		_hexParent.GlobalPosition = Map.CoordsToGlobalPosition(_coords);
		_hexParent.Rotation = 0f;

		foreach(AOEHex aoeHex in pattern.Hexes)
		{
			PackedScene hexScene = null;
			switch(aoeHex.Type)
			{
				case AOEHexType.Red:
					hexScene = _redHexScene;
					break;
				case AOEHexType.Gray:
					_hasGrayHex = true;
					hexScene = _grayHexScene;
					break;
				case AOEHexType.Empty:
					hexScene = _emptyHexScene;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			AOEHexView hexView = hexScene.Instantiate<AOEHexView>();
			_hexParent.AddChild(hexView);
			hexView.Init(aoeHex);
			hexView.SetCoords(_coords + aoeHex.LocalCoords);
			hexView.PressedEvent += OnHexPressed;

			if(!_hasGrayHex && _forcedOriginHex == null)
			{
				hexView.DraggedEvent += OnHexDragged;
			}

			Hexes.Add(hexView);
		}

		if(_forcedOriginHex != null)
		{
			SetCoords(forcedOriginHex.Coords);
		}
		else if(!_hasGrayHex)
		{
			List<Hex> possibleHexes = new List<Hex>();
			RangeHelper.FindHexesInRange(_performer.Hex, _range, true, possibleHexes);

			GameController.Instance.HexIndicatorManager.StartSettingIndicators();
			foreach(Hex hex in possibleHexes)
			{
				GameController.Instance.HexIndicatorManager.SetIndicator(hex, HexIndicatorType.Normal, OnIndicatorPressed);
			}

			GameController.Instance.HexIndicatorManager.EndSettingIndicators();

			foreach(Hex hex in possibleHexes)
			{
				_possibleHexes.Add(hex.Coords);
			}
		}

		SetProcessInput(true);
	}

	public void Close()
	{
		foreach(AOEHexView hexView in Hexes)
		{
			hexView.Destroy();
		}

		Hexes.Clear();

		_possibleHexes.Clear();

		GameController.Instance.HexIndicatorManager.ClearIndicators();

		SetProcessInput(false);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if(@event is InputEventKey inputEventKey && inputEventKey.Pressed)
		{
			if(inputEventKey.Keycode == Key.R)
			{
				if(Hexes.Count > 0)
				{
					OnHexPressed(Hexes.First());
				}
			}
		}
	}

	private void OnHexPressed(AOEHexView hexView)
	{
		if(!_hasGrayHex && _forcedOriginHex == null)
		{
			// Rotate around the clicked hex
			Vector2I delta = hexView.GlobalCoords - _coords;
			foreach(AOEHexView otherHexView in Hexes)
			{
				otherHexView.GlobalPosition -= Map.CoordsToGlobalPosition(delta);
			}

			_coords += delta;

			_moveTween?.Kill();
			Vector2 targetPosition = Map.CoordsToGlobalPosition(_coords);
			_hexParent.GlobalPosition = targetPosition;
		}

		foreach(AOEHexView otherHexView in Hexes)
		{
			Vector2I localCoords = otherHexView.GlobalCoords - _coords;
			localCoords = Map.RotateCoordsClockwise(localCoords, 1);
			otherHexView.SetCoords(_coords + localCoords);
		}

		_rotationIndex++;
		float targetDegrees = _rotationIndex * 60f;
		_hexParent.TweenRotationDegrees(targetDegrees, 0.08f).Play();

		// if(!ValidateHexes())
		// {
		// 	GD.PrintErr("Rotating AOE didn't work properly!");
		// 	return;
		// }
		//
		// TweenPosition();

		// AOEChangedEvent?.Invoke();

		if(!_hasGrayHex)
		{
			SetCoords(_coords);
		}
	}

	private void OnHexDragged(AOEHexView hexView, Vector2I delta)
	{
		SetCoords(_coords + delta);
	}

	private void OnIndicatorPressed(HexIndicator hexIndicator)
	{
		if(Hexes.Any(hex => hex.GlobalCoords == hexIndicator.Hex.Coords))
		{
			return;
		}

		SetCoords(hexIndicator.Hex.Coords);
	}

	private void SetCoords(Vector2I coords, bool skipAnimation = false)
	{
		Vector2I oldCoords = _coords;
		SetCoordsData(coords);
		if(!ValidateHexes())
		{
			SetCoordsData(oldCoords);
		}
		else
		{
			TweenPosition(skipAnimation);
			AOEChangedEvent?.Invoke();
		}
	}

	private void SetCoordsData(Vector2I coords)
	{
		Vector2I delta = coords - _coords;
		if(delta == Vector2I.Zero)
		{
			return;
		}

		_coords = coords;

		foreach(AOEHexView hexView in Hexes)
		{
			hexView.SetCoords(hexView.GlobalCoords + delta);
		}
	}

	private bool ValidateHexes()
	{
		if(_forcedOriginHex != null)
		{
			return true;
		}
		else if(_hasGrayHex)
		{
			// Gray hex needs to be centered on performer
			if(_coords != _performer.Hex.Coords)
			{
				Log.Write("AOE has been moved while this was not allowed!");
				SetCoordsData(_performer.Hex.Coords);
			}

			return true;
		}

		if(IsInRange())
		{
			return true;
		}

		_coordsCache.Clear();
		RangeHelper.FindCoordsInRange(_coords, 2, _coordsCache);
		_coordsCache.RemoveAt(0);

		foreach(Vector2I coords in _coordsCache)
		{
			SetCoordsData(coords);

			if(IsInRange())
			{
				return true;
			}
		}

		return false;

		bool IsInRange()
		{
			return Hexes.Any(hex => _possibleHexes.Contains(hex.GlobalCoords)); // Map.Distance(_performer.Hex.Coords, hex.GlobalCoords) <= _range && );
		}
	}

	private void TweenPosition(bool skipAnimation)
	{
		_moveTween?.Kill();
		Vector2 targetPosition = Map.CoordsToGlobalPosition(_coords);
		_moveTween = _hexParent.TweenGlobalPosition(targetPosition, 0.05f).Play(skipAnimation);
	}
}