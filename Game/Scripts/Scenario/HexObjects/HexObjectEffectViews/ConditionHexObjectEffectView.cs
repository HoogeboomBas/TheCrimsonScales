using Godot;

public partial class ConditionHexObjectEffectView : HexObjectEffectView<ConditionHexObjectEffectView.Parameters>
{
	public class Parameters(ConditionModel conditionModel) : HexObjectEffectViewParameters
	{
		public override string ScenePath => "res://Scenes/Scenario/HexObjectEffectViews/ConditionHexObjectEffectView.tscn";

		public ConditionModel ConditionModel { get; } = conditionModel;
	}

	[Export]
	private ConditionView _conditionView;

	protected override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_conditionView.SetCondition(parameters.ConditionModel);
	}

	public void SetStackCount(int count)
	{
		_conditionView.SetStackText(count > 1 ? count.ToString() : null);
	}
}