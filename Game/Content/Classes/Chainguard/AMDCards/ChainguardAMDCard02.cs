using System.Collections.Generic;

public class ChainguardAMDCard02 : ChainguardAMDCardModel
{
	protected override int AtlasIndex => 5;

	public override bool GetRolling(AttackAbility.State state) => true;

	public override int? GetValue(AttackAbility.State state) => 0;

	public override List<Ability> GetAbilities(AttackAbility.State state) =>
	[
		ShieldAbility.Builder().WithShieldValue(1).Build()
	];
}