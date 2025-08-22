using System.Collections.Generic;

public class CopperneckBerries : MirefootCardModel<CopperneckBerries.CardTop, CopperneckBerries.CardBottom>
{
	public override string Name => "Copperneck Berries";
	public override int Level => 1;
	public override int Initiative => 70;
	protected override int AtlasIndex => 12;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new HealAbility(4, range: 1))
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build()),
			new AbilityCardAbility(new ConditionAbility([Conditions.Strengthen, Conditions.Poison1],
				onAbilityEnded: async abilityState =>
				{
					if(abilityState.Performed)
					{
						await AbilityCmd.GainXP(abilityState.Performer, 1);
					}
				}
			))
		];
	}
}