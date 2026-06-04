using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class TeleportPrompt(
	TeleportAbility.State teleportAbilityState, Figure performer, EffectCollection effectCollection, Func<string> getHintText, bool forcedMovement = false,
	Action<TeleportAbility.State, List<Hex>> customHexes = null, Func<TeleportAbility.State, Hex, bool> filterHexes = null)
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

		List<Hex> allHexesInRange = [];

		if(customHexes == null)
		{
			allHexesInRange.AddRange(GameController.Instance.Map.Hexes.Values
				.Where(hex => hex.Revealed &&
					Map.SimpleDistance(hex.Coords, performer.Hex.Coords) < teleportAbilityState.Distance));
		}
		else
		{
			customHexes(teleportAbilityState, allHexesInRange);
		}

		foreach(Hex hex in allHexesInRange)
		{
			if(MoveHelper.CanPass(teleportAbilityState, performer, hex, forcedMovement) &&
				MoveHelper.CanStopAt(teleportAbilityState, performer, hex))
			{
				_possibleHexes.Add(hex);
			}
		}

		if(filterHexes != null)
		{
			for(int i = _possibleHexes.Count - 1; i >= 0; i--)
			{
				Hex possibleHex = _possibleHexes[i];
				if(!filterHexes(teleportAbilityState, possibleHex))
				{
					_possibleHexes.RemoveAt(i);
				}
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