public class PlusOneRangeEnhancement : PlusOneEnhancement<TargetedAbilityState>
{
	public override int BaseCost => 30;

	protected override void _Enhance(TargetedAbilityState state, EnhancementMark enhancementMark)
	{
		state.AbilityAdjustRange(1);
	}
}