public class PlusOneSummonAttackEnhancement : PlusOneEnhancement<SummonAbility.State>
{
	public override int BaseCost => 100;

	protected override void _Enhance(SummonAbility.State state, EnhancementMark enhancementMark)
	{
		state.AdjustAttack(1);
	}
}