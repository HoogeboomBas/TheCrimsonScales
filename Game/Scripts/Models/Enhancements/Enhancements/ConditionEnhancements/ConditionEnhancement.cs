public abstract class ConditionEnhancement<T> : EnhancementModel<IConditionsAbilityState>
	where T : ConditionModel
{
	public override string TexturePath => Icons.GetCondition(ModelDB.Condition<T>());

	public override bool DefaultDoubleCostOnDoubleTarget => true;

	protected override void _Enhance(IConditionsAbilityState state, EnhancementMark enhancementMark)
	{
		state.AbilityAddCondition(ModelDB.Condition<T>());
	}
}