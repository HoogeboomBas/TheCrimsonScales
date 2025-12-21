public class PlusTwoAMDCard : DefaultAMDCardModel
{
	protected override int AtlasIndex => 17;

	public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
}