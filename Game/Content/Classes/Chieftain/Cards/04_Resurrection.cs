using System.Collections.Generic;

public class Resurrection : ChieftainCardModel<Resurrection.CardTop, Resurrection.CardBottom>
{
	public override string Name => "Resurrection";
	public override int Level => 1;
	public override int Initiative => 32;
	protected override int AtlasIndex => 4;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Earth,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilitySetHasAdvantage();

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}, advantage")
					)
				)
				.Build()
			),
		];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					IEnumerable<AbilityCard> selectedAbilityCards =
						await AbilityCmd.SelectAbilityCards((Character)state.Performer, CardState.Lost, 0, 3,
							hintText: $"Select up to 3 lost cards to recover");

					foreach(AbilityCard selectedAbilityCard in selectedAbilityCards)
					{
						await AbilityCmd.ReturnToHand(selectedAbilityCard);

						state.SetPerformed();
					}
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];

		public override bool Loss => true;
		public override bool Unrecoverable => true;
	}
}