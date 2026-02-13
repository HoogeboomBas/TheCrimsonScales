public class PlusOneHealEnhancement : PlusOneEnhancement<HealAbility.State>
{
	public override int BaseCost => 30;

	protected override void _Enhance(HealAbility.State state, EnhancementMark enhancementMark)
	{
		state.AbilityAdjustHealValue(1);
	}
}