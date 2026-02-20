public abstract class PlusOneEnhancement<TState> : EnhancementModel<TState>, IPlusOneEnhancement
	where TState : AbilityState
{
	public override string TexturePath => Icons.PlusOneEnhancement;

	public override bool DefaultDoubleCostOnDoubleTarget => true;
}