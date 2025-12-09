using System.Collections.Generic;

public class ChainguardAMDCard07 : ChainguardAMDCardModel
{
	protected override int AtlasIndex => 14;

	public override int? GetValue(AttackAbility.State state) => 1;

	public override List<Ability> GetAbilities(AttackAbility.State state) =>
	[
		CreateTrapAbility.Builder().WithDamage(2).WithRange(2).Build()
	];
}