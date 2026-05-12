using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class MonsterTeleportPrompt(
	TeleportAbility.State teleportAbilityState, Figure performer, EffectCollection effectCollection, Func<string> getHintText,
	Action<TeleportAbility.State, List<Hex>> customHexes = null, Func<TeleportAbility.State, Hex, bool> filterHexes = null)
	: Prompt<MonsterTeleportPrompt.Answer>(effectCollection, getHintText)
{
	public class Answer : PromptAnswer
	{
		public Vector2I DestinationCoords { get; init; }
	}

	private readonly List<Hex> _possibleHexes = new List<Hex>();
	private Hex _selectedHex;

	protected override bool CanConfirm => _selectedHex != null;

	protected override bool CanSkip => _possibleHexes.Count == 0;

	protected override void Enable()
	{
		base.Enable();

		_possibleHexes.Clear();

		if(customHexes != null)
		{
			customHexes(teleportAbilityState, _possibleHexes);
		}
		else
		{
			foreach(Hex hex in GameController.Instance.Map.Hexes.Values.Where(hex => hex.Revealed))
			{
				int distance = Map.SimpleDistance(hex.Coords, performer.Hex.Coords);
				if(distance <= teleportAbilityState.Distance && MoveHelper.CanStopAt(teleportAbilityState, performer, hex))
				{
					_possibleHexes.Add(hex);
				}
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

		if(_possibleHexes.Count == 0)
		{
			Skip();
			return;
		}

		if(_possibleHexes.Count == 1)
		{
			_selectedHex = _possibleHexes.First();
			Complete(true);
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