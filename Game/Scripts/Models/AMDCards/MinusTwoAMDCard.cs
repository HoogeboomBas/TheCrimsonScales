public class MinusTwoAMDCard : DefaultAMDCardModel
{
	protected override int AtlasIndex => 16;

	public override int? GetValue(AttackAbility.State attackAbilityState) => -2;
}