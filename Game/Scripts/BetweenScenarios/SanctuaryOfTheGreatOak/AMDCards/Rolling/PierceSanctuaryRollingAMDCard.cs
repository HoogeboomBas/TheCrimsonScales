public class PierceSanctuaryRollingAMDCard : SanctuaryRollingAMDCardModel
{
	protected override int AtlasIndex => 6;

	public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

	public override int? Pierce => 3;
}