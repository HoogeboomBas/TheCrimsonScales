public class PlusOneTeleportEnhancement : PlusOneEnhancement<TeleportAbility.State>
{
	public override int BaseCost => 50;

	protected override void _Enhance(TeleportAbility.State state, EnhancementMark enhancementMark)
	{
		state.AdjustDistance(1);
	}
}