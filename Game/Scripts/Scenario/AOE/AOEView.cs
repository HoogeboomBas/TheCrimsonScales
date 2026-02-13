using System;
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
	private PackedScene _yellowHexScene;
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
	private GTween _rotateTween;

	public List<AOEHexView> Hexes { get; } = new List<AOEHexView>();

	public event Action AOEChangedEvent;

	public override void _Ready()
	{
		base._Ready();

		GameController.Instance.AOEButtonView.MirrorPressed += OnMirrorPressed;
		GameController.Instance.AOEButtonView.RotateCounterClockwisePressed += OnRotateCounterClockwisePressed;
		GameController.Instance.AOEButtonView.RotateClockwisePressed += OnRotateClockwisePressed;
	}

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

		foreach(AOEHex aoeHex in pattern.LocalHexes)
		{
			PackedScene hexScene = null;
			if(aoeHex.Type.HasFlag(AOEHexType.Red))
			{
				hexScene = _redHexScene;
			}
			else if(aoeHex.Type.HasFlag(AOEHexType.Gray))
			{
				_hasGrayHex = true;
				hexScene = _grayHexScene;
			}
			else if(aoeHex.Type.HasFlag(AOEHexType.Yellow))
			{
				hexScene = _yellowHexScene;
			}
			else if(aoeHex.Type.HasFlag(AOEHexType.Empty))
			{
				hexScene = _emptyHexScene;
			}

			AOEHexView hexView = hexScene.Instantiate<AOEHexView>();
			_hexParent.AddChild(hexView);
			hexView.Init(aoeHex);
			hexView.SetCoords(_coords + aoeHex.Coords);
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

		GameController.Instance.AOEButtonView.Open(!CheckSymmetry(pattern));

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
		GameController.Instance.AOEButtonView.Close();

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
		Rotate(hexView.GlobalCoords);
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

	private void OnMirrorPressed()
	{
		// Make sure all animations are finished, otherwise mirroring goes horribly wrong
		_moveTween?.Complete();
		_rotateTween?.Complete();

		foreach(AOEHexView hex in Hexes)
		{
			Vector2I localCoords = hex.GlobalCoords - _coords;
			localCoords = Map.MirrorCoords(localCoords);
			hex.SetCoords(_coords + localCoords);
			hex.SetGlobalPosition(Map.CoordsToGlobalPosition(hex.GlobalCoords));
		}

		SetCoords(_coords);
	}

	private void OnRotateCounterClockwisePressed()
	{
		Rotate(_coords, false);
	}

	private void OnRotateClockwisePressed()
	{
		Rotate(_coords);
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

	private void Rotate(Vector2I rotationCoords, bool clockwise = true)
	{
		if(!_hasGrayHex && _forcedOriginHex == null)
		{
			// Rotate around the clicked hex
			Vector2I delta = rotationCoords - _coords;
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
			localCoords = Map.RotateCoordsClockwise(localCoords, clockwise ? 1 : 5);
			otherHexView.SetCoords(_coords + localCoords);
		}

		_rotationIndex += clockwise ? 1 : 5;
		_rotationIndex %= 6;
		float targetDegrees = _rotationIndex * 60f;
		_rotateTween?.Kill();
		_rotateTween = _hexParent.TweenRotationDegrees(targetDegrees, 0.08f).Play();

		if(!_hasGrayHex)
		{
			SetCoords(_coords);
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
			return Hexes.Any(hex => _possibleHexes.Contains(hex.GlobalCoords));
		}
	}

	private bool CheckSymmetry(AOEPattern pattern)
	{
		if(pattern.LocalHexes.Count == 0)
		{
			return true;
		}

		// Check if the AOE pattern is symmetrical, by mirroring it, and then rotating it 6 times and checking if it ever matches the original
		AOEPattern checkPattern = new AOEPattern(pattern.LocalHexes.Select(hex => new AOEHex(Map.MirrorCoords(hex.Coords), hex.Type)).ToList());
		Vector2I pivotOffset = pattern.LocalHexes[0].Coords;
		for(int i = 0; i < 6; i++)
		{
			// Go through each hex of the check pattern and offset it to overlap the pivot
			foreach(AOEHex pivotCheckHex in checkPattern.LocalHexes)
			{
				Vector2I checkOffset = pivotCheckHex.Coords - pivotOffset;

				bool symmetryFound = true;

				// Go through each hex of the original pattern and check if it is represented in the check pattern
				foreach(AOEHex hex in pattern.LocalHexes)
				{
					bool matchFound = false;
					foreach(AOEHex checkHex in checkPattern.LocalHexes)
					{
						Vector2I checkHexGlobalCoords = checkHex.Coords - checkOffset;
						if(hex.Coords == checkHexGlobalCoords && hex.Type == checkHex.Type)
						{
							matchFound = true;
						}
					}

					if(!matchFound)
					{
						symmetryFound = false;
						break;
					}
				}

				if(symmetryFound)
				{
					return true;
				}
			}

			checkPattern = new AOEPattern(checkPattern.LocalHexes.Select(hex => new AOEHex(Map.RotateCoordsClockwise(hex.Coords, 1), hex.Type))
				.ToList());
		}

		return false;
	}

	private void TweenPosition(bool skipAnimation)
	{
		_moveTween?.Kill();
		Vector2 targetPosition = Map.CoordsToGlobalPosition(_coords);
		_moveTween = _hexParent.TweenGlobalPosition(targetPosition, 0.05f).Play(skipAnimation);
	}
}