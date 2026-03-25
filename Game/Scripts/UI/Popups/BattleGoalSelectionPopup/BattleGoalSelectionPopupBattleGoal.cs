using System;
using Godot;

public partial class BattleGoalSelectionPopupBattleGoal : Control
{
	[Export]
	private BattleGoalToggleButton _battleGoalToggleButton;
	[Export]
	private BetterButton _betterButton;

	public BattleGoalModel BattleGoalModel => _battleGoalToggleButton.BattleGoalModel;

	public event Action<BattleGoalSelectionPopupBattleGoal> PressedEvent;

	public override void _Ready()
	{
		base._Ready();

		_betterButton.Pressed += OnPressed;
	}

	public void Init(BattleGoalModel battleGoalModel)
	{
		_battleGoalToggleButton.Init(battleGoalModel);
	}

	public void SetSelected(bool active)
	{
		_battleGoalToggleButton.SetSelected(active, true);
	}

	private void OnPressed()
	{
		PressedEvent?.Invoke(this);
	}
}