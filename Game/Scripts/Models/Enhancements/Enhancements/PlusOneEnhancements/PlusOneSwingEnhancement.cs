public class PlusOneSwingEnhancement : PlusOneEnhancement<TargetedAbilityState>
{
	public override int BaseCost => 30;

	protected override void _Enhance(TargetedAbilityState state, EnhancementMark enhancementMark)
	{
		state.AbilityAdjustSwing(1);
	}
}