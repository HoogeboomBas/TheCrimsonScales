using System.Collections.Generic;

public class PartyAMDCard1 : PartyAMDCardModel
{
	protected override int AtlasIndex => 0;

	public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
	[
		OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await AbilityCmd.SufferDamage(state, attackAbilityState.Target, 1);
			})
			.Build(),

		HealAbility.Builder()
			.WithHealValue(1)
			.WithTarget(Target.Self)
			.Build()
	];
}