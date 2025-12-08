using System.Collections.Generic;

public class ChieftainAMDCard03 : ChieftainAMDCardModel
{
	protected override int AtlasIndex => 3;

	public override int? GetValue(AttackAbility.State state) => -2;

	public override List<Ability> GetAbilities(AttackAbility.State state) => 
	[
		ConditionAbility.Builder().WithConditions(Conditions.Bless).WithTarget(Target.Self).Build()
	];
}