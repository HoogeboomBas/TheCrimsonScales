using Godot;

public partial class BattleGoalProgressUpdateView : Control
{
	[Export]
	private PackedScene _battleGoalProgressViewScene;
	[Export]
	private Control _itemParent;

	public void AddItem(BattleGoal battleGoal)
	{
		BattleGoalProgressUpdateViewItem item = _battleGoalProgressViewScene.Instantiate<BattleGoalProgressUpdateViewItem>();
		_itemParent.AddChild(item);
		item.Init(battleGoal);
	}
}