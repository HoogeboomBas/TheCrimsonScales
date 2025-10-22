using Fractural.Tasks;
using Godot;
using GTweensGodot.Extensions;

public class ScenarioSetupPhase : ScenarioPhase
{
	private ScenarioSetupState _scenarioSetupState;

	private Character _selectedCharacter;

	private Character _draggingCharacter;
	private Hex _draggingCharacterHex;

	public override async GDTask Activate()
	{
		await base.Activate();

		_scenarioSetupState = GameController.Instance.SavedCampaign.SavedScenario.ScenarioSetupState;
		if(_scenarioSetupState != null)
		{
			// Save data contains data for this card selection phase
			//GameController.SetFastForward(true);

			_scenarioSetupState = GameController.Instance.SavedCampaign.SavedScenario.ScenarioSetupState;
		}
		else
		{
			_scenarioSetupState = CreateInitialState();
			GameController.Instance.SavedCampaign.SavedScenario.ScenarioSetupState = _scenarioSetupState;
		}

		if(!_scenarioSetupState.Completed)
		{
			GameController.SetFastForward(false);

			// Set selected character to the first in order that isn't exhausted
			foreach(Character character in GameController.Instance.CharacterManager.Characters)
			{
				if(!character.IsDead)
				{
					SetSelectedCharacter(character);
					break;
				}
			}

			foreach(PortraitViewCharacterPortrait portrait in GameController.Instance.PortraitView.CharacterPortraits)
			{
				portrait.PressedEvent += OnPortraitPressed;
			}

			GameController.Instance.ScenarioSetupButtonsView.Open(OnContinuePressed);
			GameController.Instance.ScenarioSetupButtonsView.SetButtons(true);
			GameController.Instance.UndoView.Open(this);
		}

		SyncGameWithState();

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			character.UpdateInitiative();
		}

		// Wait until the scenario setup phase is completed
		await GDTask.WaitUntil(
			() =>
				_scenarioSetupState.Completed ||
				GameController.Instance.ResignRequested ||
				GameController.Instance.CheatWinRequested,
			cancellationToken: GameController.CancellationToken);

		await GameController.Instance.CheckEarlyEnd();

		foreach(PortraitViewPortrait portrait in GameController.Instance.PortraitView.Portraits)
		{
			portrait.SetSelected(false);
			portrait.PressedEvent -= OnPortraitPressed;
		}

		GameController.Instance.ScenarioSetupButtonsView.Close();
		GameController.Instance.UndoView.Close(this);

		GameController.Instance.CardSelectionView.Close();
		GameController.Instance.HexIndicatorManager.ClearIndicators();

		// End of the phase
		await GameController.Instance.CharacterManager.RemoveCharacterStartHexes();

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			await character.OnScenarioSetupCompleted();
		}
	}

	private ScenarioSetupState CreateInitialState()
	{
		ScenarioSetupState fullState = new ScenarioSetupState
		{
			CharacterScenarioSetupStates = new CharacterScenarioSetupState[GameController.Instance.CharacterManager.Characters.Count],
			Completed = false
		};

		for(int i = 0; i < fullState.CharacterScenarioSetupStates.Length; i++)
		{
			fullState.CharacterScenarioSetupStates[i] = new CharacterScenarioSetupState
			{
				StartHexCoords = GameController.Instance.CharacterManager.GetCharacter(i).Hex.Coords
			};
		}

		return fullState;
	}

	private void SyncGameWithState()
	{
		for(int i = 0; i < _scenarioSetupState.CharacterScenarioSetupStates.Length; i++)
		{
			Character character = GameController.Instance.CharacterManager.GetCharacter(i);
			SyncCharacterWithState(character);
		}
	}

	private void SyncCharacterWithState(Character character)
	{
		CharacterScenarioSetupState state = _scenarioSetupState.CharacterScenarioSetupStates[character.Index];

		character.SetOriginHexAndRotation(GameController.Instance.Map.GetHex(state.StartHexCoords));

		UpdateHexIndicators();

		if(character == _selectedCharacter)
		{
			//UpdateCardSelectionView();
		}

		UpdateChoiceButtons();
	}

	private void SetSelectedCharacter(Character character)
	{
		_selectedCharacter = character;
		// GameController.Instance.CardSelectionView.Open(_selectedCharacter.Cards, OnCardPressed, OnInitiativePressed, OnShortRestPressed, OnLongRestPressed);
		// UpdateCardSelectionView();
		UpdateHexIndicators();

		foreach(PortraitViewPortrait portrait in GameController.Instance.PortraitView.Portraits)
		{
			portrait.SetSelected(portrait is PortraitViewCharacterPortrait characterPortrait && characterPortrait.Character == character);
		}

		GameController.Instance.ScenarioSetupButtonsView.SetCharacter(_selectedCharacter);
	}

	private void UpdateHexIndicators()
	{
		GameController.Instance.HexIndicatorManager.StartSettingIndicators();

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			if(character.IsDead)
			{
				continue;
			}

			GameController.Instance.HexIndicatorManager.SetIndicator(
				character.Hex, _selectedCharacter == character ? HexIndicatorType.Selected : HexIndicatorType.Normal,
				OnIndicatorPressed, OnIndicatorDragged, OnIndicatorDragEnd);
		}

		GameController.Instance.HexIndicatorManager.EndSettingIndicators();
	}

	private void UpdateChoiceButtons()
	{
		bool hasUnfinishedCharacter = false;
		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			if(!character.IsLocal || character.IsDestroyed)
			{
				continue;
			}
		}

		GameController.Instance.ScenarioSetupButtonsView.SetButtons(!hasUnfinishedCharacter);
	}

	private void OnIndicatorPressed(HexIndicator hexIndicator)
	{
		Character otherCharacter = hexIndicator.Hex.GetHexObjectOfType<Character>();
		if(otherCharacter != null)
		{
			if(otherCharacter == _selectedCharacter)
			{
			}
			else
			{
				SetSelectedCharacter(otherCharacter);
			}
		}
	}

	private void OnIndicatorDragged(HexIndicator hexIndicator, Vector2 previousPosition, Vector2 currentPosition)
	{
		Vector2 globalMousePosition = GameController.Instance.Map.GetGlobalMousePosition();

		if(hexIndicator.Hex.TryGetHexObjectOfType(out Character character))
		{
			_draggingCharacter = character;
			_draggingCharacter.ZIndex = 100;

			Hex draggingHex = GameController.Instance.Map.GetHex(Map.GlobalPositionToCoords(globalMousePosition));
			if(draggingHex != null && draggingHex != _draggingCharacterHex && draggingHex.HasHexObjectOfType<CharacterStartHex>())
			{
				_draggingCharacterHex = draggingHex;
				character.TweenGlobalPosition(_draggingCharacterHex.GlobalPosition, 0.08f).Play();
			}

			GameController.Instance.CharacterStartHexMoveIndicator.Open(_draggingCharacter);

			GameController.Instance.CharacterStartHexMoveIndicator.Update(currentPosition);
		}
	}

	private void OnIndicatorDragEnd(HexIndicator hexIndicator, Vector2 position)
	{
		if(_draggingCharacter != null)
		{
			_draggingCharacter.ZIndex = _draggingCharacter.DefaultZIndex;
			if(_draggingCharacterHex != null && _draggingCharacter.Hex != _draggingCharacterHex)
			{
				if(_draggingCharacterHex.TryGetHexObjectOfType(out Character otherCharacter))
				{
					_scenarioSetupState.CharacterScenarioSetupStates[otherCharacter.Index].StartHexCoords = _draggingCharacter.Hex.Coords;
					SyncCharacterWithState(otherCharacter);
				}

				_scenarioSetupState.CharacterScenarioSetupStates[_draggingCharacter.Index].StartHexCoords = _draggingCharacterHex.Coords;
				SyncCharacterWithState(_draggingCharacter);

				UpdateHexIndicators();
			}

			_draggingCharacter = null;
			_draggingCharacterHex = null;

			GameController.Instance.CharacterStartHexMoveIndicator.Close();
		}
	}

	private void OnPortraitPressed(PortraitViewPortrait portrait)
	{
		if(portrait is not PortraitViewCharacterPortrait characterPortrait)
		{
			return;
		}

		Character otherCharacter = characterPortrait.Character;

		if(otherCharacter == _selectedCharacter)
		{
		}
		else
		{
			SetSelectedCharacter(otherCharacter);
		}
	}

	private void OnContinuePressed()
	{
		_scenarioSetupState.Completed = true;
	}

	private CharacterScenarioSetupState GetCharacterCardSelectionState(Character character)
	{
		return _scenarioSetupState.CharacterScenarioSetupStates[character.Index];
	}
}