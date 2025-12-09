public class ChainguardAMDCard01 : ChainguardAMDCardModel
{
	protected override int AtlasIndex => 3;

	public override int? GetValue(AttackAbility.State state) => state.Target.HasCondition(Chainguard.Shackle) ? 2 : 0;
}