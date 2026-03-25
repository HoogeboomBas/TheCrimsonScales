using Godot;

public partial class BattleGoalToggleButton : ToggleButton<BattleGoalToggleButton>
{
	[Export]
	private BattleGoalView _battleGoalView;

	public BattleGoalModel BattleGoalModel { get; private set; }

	public void Init(BattleGoalModel battleGoalModel)
	{
		BattleGoalModel = battleGoalModel;

		_battleGoalView.SetModel(BattleGoalModel);

		base.Init();
	}
}