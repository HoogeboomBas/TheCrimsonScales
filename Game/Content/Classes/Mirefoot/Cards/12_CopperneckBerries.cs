using System.Collections.Generic;
using Godot;

public class CopperneckBerries : MirefootCardModel<CopperneckBerries.CardTop, CopperneckBerries.CardBottom>
{
	public override string Name => "Copperneck Berries";
	public override int Level => 1;
	public override int Initiative => 70;
	protected override int AtlasIndex => 12;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(4, new HealDiamondPlus(this, new Vector2(0.4953472f, 0.29036877f)))
				.WithRange(1)
				.Build())
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6213116f, 0.626352f)))
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Strengthen, Conditions.Poison1])
				.WithOnAbilityEnded(async abilityState =>
					{
						if(abilityState.Performed)
						{
							await AbilityCmd.GainXP(abilityState.Performer, 1);
						}
					}
				)
				.Build())
		];
	}
}