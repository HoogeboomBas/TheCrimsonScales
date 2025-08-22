using System.Collections.Generic;
using Fractural.Tasks;

public class Neurotoxin : MirefootCardModel<Neurotoxin.CardTop, Neurotoxin.CardBottom>
{
	public override string Name => "Neurotoxin";
	public override int Level => 1;
	public override int Initiative => 84;
	protected override int AtlasIndex => 5;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(1, targets: 2, range: 3, rangeType: RangeType.Range, conditions: [Conditions.Poison1, Conditions.Muddle]))
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build()),
			new AbilityCardAbility(new HealAbility(3, conditions: [Conditions.Poison1],
				onAbilityEnded: async abilityState =>
				{
					if(abilityState.Performed)
					{
						await AbilityCmd.GainXP(abilityState.Performer, 1);
					}
				}))
		];
	}
}