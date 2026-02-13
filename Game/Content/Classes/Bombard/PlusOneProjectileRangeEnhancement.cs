public class PlusOneProjectileRangeEnhancement : PlusOneEnhancement<ProjectileAbility.State>
{
	public override int BaseCost => 30;

	protected override void _Enhance(ProjectileAbility.State state, EnhancementMark enhancementMark)
	{
		state.AbilityAdjustRange(1);
	}
}