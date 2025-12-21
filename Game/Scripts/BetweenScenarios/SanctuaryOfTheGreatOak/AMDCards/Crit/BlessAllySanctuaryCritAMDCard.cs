using System.Collections.Generic;

public class BlessAllySanctuaryCritAMDCard : SanctuaryCritAMDCardModel
{
	protected override int AtlasIndex => 0;

	public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
	[
		ConditionAbility.Builder().WithConditions(Conditions.Bless).WithTarget(Target.Allies).WithRange(int.MaxValue).Build()
	];
}