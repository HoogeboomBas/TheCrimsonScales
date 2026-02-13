using System.Collections.Generic;
using Godot;

public class Neurotoxin : MirefootCardModel<Neurotoxin.CardTop, Neurotoxin.CardBottom>
{
	public override string Name => "Neurotoxin";
	public override int Level => 1;
	public override int Initiative => 84;
	protected override int AtlasIndex => 5;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.3103919f, 0.29231876f)))
				.WithTargets(2)
				.WithRange(3, new RangeSquare(this, new Vector2(0.6124042f, 0.29231876f)))
				.WithRangeType(RangeType.Range)
				.WithConditions([Conditions.Poison1, Conditions.Muddle])
				.Build())
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3, new HealCircle(this, new Vector2(0.62056f, 0.64306784f)))
				.WithConditions(Conditions.Poison1)
				.WithOnAbilityEnded(async abilityState =>
				{
					if(abilityState.Performed)
					{
						await AbilityCmd.GainXP(abilityState.Performer, 1);
					}
				})
				.Build())
		];
	}
}