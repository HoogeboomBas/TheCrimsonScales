using System.Collections.Generic;

public class ChainguardAMDCard03 : ChainguardAMDCardModel
{
	protected override int AtlasIndex => 7;

	public override bool GetRolling(AttackAbility.State state) => true;
	
	public override int? GetValue(AttackAbility.State state) => 0;

	public override List<Ability> GetAbilities(AttackAbility.State state) =>
	[
		RetaliateAbility.Builder().WithRetaliateValue(1).Build()
	];
}