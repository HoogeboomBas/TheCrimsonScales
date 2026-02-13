public class PlusOneTargetEnhancement : PlusOneEnhancement<TargetedAbilityState>
{
	public override int BaseCost => 75;

	protected override void _Enhance(TargetedAbilityState state, EnhancementMark enhancementMark)
	{
		state.AdjustTargets(1);
	}
}