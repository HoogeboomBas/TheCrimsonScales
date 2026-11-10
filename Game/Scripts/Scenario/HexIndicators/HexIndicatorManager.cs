using System;
using System.Collections.Generic;
using Godot;

public class HexIndicatorManager
{
	private PackedScene _hexIndicatorScene;

	private Dictionary<Hex, HexIndicator> _hexIndicators = new Dictionary<Hex, HexIndicator>();
	private Dictionary<Hex, HexIndicator> _oldHexIndicators = new Dictionary<Hex, HexIndicator>();

	public HexIndicatorManager()
	{
		_hexIndicatorScene = SceneLoader.LoadPackedScene("res://Scenes/Scenario/HexIndicator.tscn");
	}

	public void StartSettingIndicators()
	{
		(_oldHexIndicators, _hexIndicators) = (_hexIndicators, _oldHexIndicators);
	}

	public void EndSettingIndicators()
	{
		foreach(KeyValuePair<Hex, HexIndicator> oldHexIndicator in _oldHexIndicators)
		{
			if(!_hexIndicators.ContainsKey(oldHexIndicator.Key))
			{
				oldHexIndicator.Value.Destroy();
			}
		}

		_oldHexIndicators.Clear();
	}

	public void SetIndicator(Hex hex, HexIndicatorType hexIndicatorType, Action<HexIndicator> onIndicatorPressed = null,
		Action<HexIndicator, Vector2, Vector2> onDragged = null, Action<HexIndicator, Vector2> onDragEnd = null)
	{
		if(hex == null)
		{
			Log.Warning("Cannot put a hex indicator on a null hex.");
			return;
		}

		if(!_hexIndicators.TryGetValue(hex, out HexIndicator hexIndicator))
		{
			if(!_oldHexIndicators.TryGetValue(hex, out hexIndicator))
			{
				hexIndicator = _hexIndicatorScene.Instantiate<HexIndicator>();
				GameController.Instance.Map.AddChild(hexIndicator);
				hexIndicator.Init(hex, onDragged != null);
			}

			_hexIndicators.Add(hex, hexIndicator);
		}

		hexIndicator.SetType(hexIndicatorType, onIndicatorPressed, onDragged, onDragEnd);
	}

	public void ClearIndicators()
	{
		foreach(KeyValuePair<Hex, HexIndicator> hexIndicator in _hexIndicators)
		{
			hexIndicator.Value.Destroy();
		}

		_hexIndicators.Clear();
	}
}