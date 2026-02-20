using Godot;

public partial class TrapCondition : Node2D
{
	[Export]
	private ConditionView _conditionView;

	public void Init(ConditionModel conditionModel)
	{
		_conditionView.SetCondition(conditionModel);
	}
}