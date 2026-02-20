public class PlusOneSummonHealthEnhancement : PlusOneEnhancement<SummonAbility.State>
{
	public override int BaseCost => 40;

	protected override void _Enhance(SummonAbility.State state, EnhancementMark enhancementMark)
	{
		state.AdjustHealth(1);
	}
}