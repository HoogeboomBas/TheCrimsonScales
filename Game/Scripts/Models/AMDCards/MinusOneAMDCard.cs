public class MinusOneAMDCard : DefaultAMDCardModel
{
	protected override int AtlasIndex => 11;

	public override int? GetValue(AttackAbility.State attackAbilityState) => -1;
}