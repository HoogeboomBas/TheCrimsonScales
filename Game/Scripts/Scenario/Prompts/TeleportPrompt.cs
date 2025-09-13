using System;
using System.Collections.Generic;
using Godot;

public class TeleportPrompt(TeleportAbility.State teleportAbilityState, Figure performer, EffectCollection effectCollection, Func<string> getHintText)
	: Prompt<TeleportPrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public Vector2I DestinationCoords { get; init; }
	}

	private readonly List<Hex> _possibleHexes = new List<Hex>();
	private Hex _selectedHex;

	protected override bool CanConfirm => _selectedHex != null;

	protected override bool CanSkip => true;

	protected override void Enable()
	{
		base.Enable();

		_possibleHexes.Clear();

		foreach((Vector2I coords, Hex hex) in GameController.Instance.Map.Hexes)
		{
			if(!hex.Revealed)
			{
				continue;
			}

			int distance = Map.SimpleDistance(coords, performer.Hex.Coords);
			if(distance <= teleportAbilityState.Distance && MoveHelper.CanStopAt(performer, hex))
			{
				_possibleHexes.Add(hex);
			}
		}
	}

	protected override void UpdateState()
	{
		base.UpdateState();

		GameController.Instance.HexIndicatorManager.StartSettingIndicators();

		foreach(Hex hex in _possibleHexes)
		{
			GameController.Instance.HexIndicatorManager.SetIndicator(hex, HexIndicatorType.Normal, OnIndicatorPressed);
		}

		if(_selectedHex != null)
		{
			GameController.Instance.HexIndicatorManager.SetIndicator(_selectedHex, HexIndicatorType.Selected, OnIndicatorPressed);
		}

		GameController.Instance.HexIndicatorManager.EndSettingIndicators();

		GameController.Instance.TeleportPath.Open(performer.Hex, _selectedHex);
	}

	protected override void Disable()
	{
		base.Disable();

		GameController.Instance.HexIndicatorManager.ClearIndicators();
		GameController.Instance.TeleportPath.Close();
	}

	private void OnIndicatorPressed(HexIndicator hexIndicator)
	{
		if(_selectedHex == hexIndicator.Hex)
		{
			_selectedHex = null;
		}
		else
		{
			_selectedHex = hexIndicator.Hex;
		}

		FullUpdateState();
	}

	protected override Answer CreateAnswer()
	{
		return new Answer
		{
			DestinationCoords = _selectedHex.Coords
		};
	}
}