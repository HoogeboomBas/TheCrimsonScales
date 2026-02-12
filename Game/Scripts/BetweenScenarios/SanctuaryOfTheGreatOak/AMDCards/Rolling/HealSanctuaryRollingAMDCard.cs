using System.Collections.Generic;

public class HealSanctuaryRollingAMDCard : SanctuaryRollingAMDCardModel
{
	protected override int AtlasIndex => 2;

	public override int? GetValue(AttackAbility.State attackAbilityState) => 1;

	public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
	[
		HealAbility.Builder().WithHealValue(1).WithRange(2).Build()
	];
}