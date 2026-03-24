using System;
using System.Collections.Generic;
using Godot;

public partial class ScenarioSetupButtonsView : Control
{
	[Export]
	private ChoiceButton _startScenarioButton;

	[Export]
	private ChoiceButton _equipmentButton;
	[Export]
	private ChoiceButton _cardsButton;
	[Export]
	private ChoiceButton _battleGoalsButton;

	[Export]
	private ExclamationMark _battleGoalsExclamationMark;

	private readonly List<object> _blockers = new List<object>();

	private bool _startActive;
	private bool _opened;

	private Character _selectedCharacter;

	private event Action StartPressedEvent;
	private event Action<Character, int> BattleGoalSelectedEvent;

	public override void _Ready()
	{
		base._Ready();

		_startScenarioButton.BetterButton.Pressed += OnStartScenarioPressed;

		_equipmentButton.BetterButton.Pressed += OnEquipmentPressed;
		_cardsButton.BetterButton.Pressed += OnCardsPressed;
		_battleGoalsButton.BetterButton.Pressed += OnBattleGoalsPressed;
	}

	public void Open(Action onStartPressed, Action<Character, int> battleGoalSelectedEvent)
	{
		_opened = true;

		StartPressedEvent = onStartPressed;
		BattleGoalSelectedEvent = battleGoalSelectedEvent;

		_startScenarioButton.SetActive(false);

		_equipmentButton.SetActive(true);
		_cardsButton.SetActive(true);
		_battleGoalsButton.SetActive(true);
	}

	public void SetCharacter(Character character)
	{
		_selectedCharacter = character;

		UpdateButtons();
	}

	public void SetButtons(bool startActive)
	{
		_startActive = startActive;

		UpdateButtons();
	}

	public void Close()
	{
		_opened = false;

		_startActive = false;

		UpdateButtons();
	}

	public void Block(object blocker)
	{
		_blockers.AddIfNew(blocker);

		UpdateButtons();
	}

	public void UnBlock(object blocker)
	{
		_blockers.Remove(blocker);

		UpdateButtons();
	}

	private void UpdateButtons()
	{
		bool blocked = _blockers.Count > 0;

		_startScenarioButton.SetActive(_startActive && !blocked);

		_equipmentButton.SetActive(_opened && !blocked);
		_cardsButton.SetActive(_opened && !blocked);
		_battleGoalsButton.SetActive(_opened && !blocked);

		_battleGoalsExclamationMark.SetActive(_selectedCharacter != null && _selectedCharacter.SelectedBattleGoalModel == null);
	}

	private void OnStartScenarioPressed()
	{
		if(!_startActive)
		{
			return;
		}

		StartPressedEvent?.Invoke();
	}

	private void OnEquipmentPressed()
	{
		if(!_opened)
		{
			return;
		}

		AppController.Instance.PopupManager.RequestPopup(new EquipmentPopup.Request
		{
			SavedCharacter = _selectedCharacter.SavedCharacter
		});
	}

	private void OnCardsPressed()
	{
		if(!_opened)
		{
			return;
		}

		AppController.Instance.PopupManager.RequestPopup(new CardSelectionPopup.Request
		{
			SavedCharacter = _selectedCharacter.SavedCharacter
		});
	}

	private void OnBattleGoalsPressed()
	{
		if(!_opened)
		{
			return;
		}

		AppController.Instance.PopupManager.RequestPopup(new BattleGoalSelectionPopup.Request
		{
			Character = _selectedCharacter,
			BattleGoalSelectedEvent = BattleGoalSelectedEvent
		});
	}
}