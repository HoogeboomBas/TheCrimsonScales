public class ChieftainAMDCard06 : ChieftainAMDCardModel
{
	protected override int AtlasIndex => 6;

	public override bool GetRolling(AttackAbility.State state) => state.Performer is Summon;

	public override int? GetValue(AttackAbility.State state) => 1;
}