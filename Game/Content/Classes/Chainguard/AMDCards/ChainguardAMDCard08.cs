using System.Collections.Generic;

public class ChainguardAMDCard08 : ChainguardAMDCardModel
{
	protected override int AtlasIndex => 8;

	public override bool GetRolling(AttackAbility.State state) => true;

	public override int? GetValue(AttackAbility.State state) => 0;

	public override List<Ability> GetAbilities(AttackAbility.State state) =>
	[
		HealAbility.Builder().WithHealValue(1).Build()
	];
}