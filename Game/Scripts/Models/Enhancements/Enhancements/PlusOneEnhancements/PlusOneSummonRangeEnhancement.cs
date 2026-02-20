public class PlusOneSummonRangeEnhancement : PlusOneEnhancement<SummonAbility.State>
{
	public override int BaseCost => 50;

	protected override void _Enhance(SummonAbility.State state, EnhancementMark enhancementMark)
	{
		state.AdjustRange(1);
	}
}