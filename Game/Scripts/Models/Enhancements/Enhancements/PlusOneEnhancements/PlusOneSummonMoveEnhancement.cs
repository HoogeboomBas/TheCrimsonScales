public class PlusOneSummonMoveEnhancement : PlusOneEnhancement<SummonAbility.State>
{
	public override int BaseCost => 60;

	protected override void _Enhance(SummonAbility.State state, EnhancementMark enhancementMark)
	{
		state.AdjustMove(1);
	}
}