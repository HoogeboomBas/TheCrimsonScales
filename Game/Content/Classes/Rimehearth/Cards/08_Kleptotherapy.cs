using System.Collections.Generic;
using Godot;

public class Kleptotherapy : RimehearthCardModel<Kleptotherapy.CardTop, Kleptotherapy.CardBottom>
{
	public override string Name => "Kleptotherapy";
	public override int Level => 1;
	public override int Initiative => 48;
	protected override int AtlasIndex => 8;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.3049949f, 0.23268698f)))
				.WithRange(2)
				.WithRangeType(RangeType.Melee)
				.WithConditions([Conditions.Wound1, Conditions.Chill])
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
						applyFunction: async applyParameters =>
						{
							await AbilityCmd.InfuseElement(applyParameters.AbilityState, [Element.Fire, Element.Ice]);

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetElement([Element.Fire, Element.Ice]))}")
					)
				)
				.Build()),
		];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(state.LootedCoinCount >= 2)
					{
						await AbilityCmd.InfuseElement(state, Element.Fire);
					}

					if(state.LootedCoinCount <= 2)
					{
						await AbilityCmd.InfuseElement(state, Element.Ice);
					}
				})
				.Build())
		];
	}
}