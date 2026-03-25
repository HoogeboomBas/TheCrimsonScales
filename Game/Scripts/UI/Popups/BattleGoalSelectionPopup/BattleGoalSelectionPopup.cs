using System;
using System.Collections.Generic;
using Godot;

public partial class BattleGoalSelectionPopup : Popup<BattleGoalSelectionPopup.Request>
{
	public class Request : PopupRequest
	{
		public Character Character { get; init; }
		public Action<Character, int> BattleGoalSelectedEvent { get; init; }
	}

	[Export]
	private PackedScene _battleGoalScene;
	[Export]
	private Control _battleGoalsParent;

	[Export]
	private BetterButton _confirmButton;

	private readonly List<BattleGoalSelectionPopupBattleGoal> _battleGoals = new List<BattleGoalSelectionPopupBattleGoal>();

	public override void _Ready()
	{
		base._Ready();

		_confirmButton.Pressed += OnConfirmPressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		foreach(BattleGoalModel battleGoalModel in PopupRequest.Character.AvailableBattleGoals)
		{
			BattleGoalSelectionPopupBattleGoal battleGoal = _battleGoalScene.Instantiate<BattleGoalSelectionPopupBattleGoal>();
			_battleGoalsParent.AddChild(battleGoal);
			battleGoal.Init(battleGoalModel);
			battleGoal.PressedEvent += OnBattleGoalPressed;
			_battleGoals.Add(battleGoal);

			if(PopupRequest.Character.SelectedBattleGoalModel != null)
			{
				battleGoal.SetSelected(battleGoal.BattleGoalModel == PopupRequest.Character.SelectedBattleGoalModel);
			}
		}

		PopupRequest.Character.BattleGoalChangedEvent += OnBattleGoalChanged;
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(BattleGoalSelectionPopupBattleGoal battleGoal in _battleGoals)
		{
			battleGoal.QueueFree();
		}

		_battleGoals.Clear();

		if(PopupRequest.Character != null)
		{
			PopupRequest.Character.BattleGoalChangedEvent -= OnBattleGoalChanged;
		}
	}

	private void OnBattleGoalPressed(BattleGoalSelectionPopupBattleGoal battleGoal)
	{
		if(battleGoal.BattleGoalModel == PopupRequest.Character.SelectedBattleGoalModel)
		{
			return;
		}

		// PopupRequest.Character.SetBattleGoal(battleGoal.BattleGoalModel);
		//
		// foreach(BattleGoalSelectionPopupBattleGoal otherBattleGoal in _battleGoals)
		// {
		// 	otherBattleGoal.SetSelected(otherBattleGoal == battleGoal);
		// }

		PopupRequest.BattleGoalSelectedEvent?.Invoke(
			PopupRequest.Character,
			PopupRequest.Character.AvailableBattleGoals.IndexOf(battleGoal.BattleGoalModel));
	}

	private void OnConfirmPressed()
	{
		Close();
	}

	private void OnBattleGoalChanged(Character character)
	{
		foreach(BattleGoalSelectionPopupBattleGoal battleGoal in _battleGoals)
		{
			battleGoal.SetSelected(battleGoal.BattleGoalModel == character.SelectedBattleGoalModel);
		}
	}
}