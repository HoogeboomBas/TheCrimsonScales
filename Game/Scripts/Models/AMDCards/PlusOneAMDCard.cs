public class PlusOneAMDCard : DefaultAMDCardModel
{
	protected override int AtlasIndex => 6;

	public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
}