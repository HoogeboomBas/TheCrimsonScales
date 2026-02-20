public class PlusOneRetaliateEnhancement : PlusOneEnhancement<RetaliateAbility.State>
{
	public override int BaseCost => 60;

	public override bool DefaultTripleCostOnPersistent => true;

	protected override void _Enhance(RetaliateAbility.State state, EnhancementMark enhancementMark)
	{
		state.AdjustRetaliateValue(1);
	}
}