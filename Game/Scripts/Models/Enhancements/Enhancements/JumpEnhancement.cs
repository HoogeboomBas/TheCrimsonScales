public class JumpEnhancement : EnhancementModel<MoveAbility.State>
{
	public override string TexturePath => Icons.JumpEnhancement;
	public override int BaseCost => 60;

	protected override void _Enhance(MoveAbility.State state, EnhancementMark enhancementMark)
	{
		state.AddJump();
	}
}