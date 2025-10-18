using System.Collections.Generic;
using System.Linq;

public class EncouragedConviction : HierophantLevelUpCardModel<EncouragedConviction.CardTop, EncouragedConviction.CardBottom>
{
	public override string Name => "Encouraged Conviction";
	public override int Level => 3;
	public override int Initiative => 14;
	protected override int AtlasIndex => 15 - 2;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState =>
					[
						HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build(),
						ShieldAbility.Builder().WithShieldValue(1).Build(),
						RetaliateAbility.Builder()
							.WithRetaliateValue(1)
							.WithAbilityStartedSubscription(
								ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Earth,
									parameters => true,
									async parameters =>
									{
										RetaliateAbility.State retaliateAbilityState = (RetaliateAbility.State)parameters.AbilityState;
										retaliateAbilityState.AdjustRange(2);
										await AbilityCmd.GainXP(grantAbilityState.Performer, 1);
									}
								)
							)
							.Build()
					]
				)
				.WithRange(3)
				.Build())
		];

		protected override bool Round => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
					{
						Figure figure = await AbilityCmd.SelectFigure(state,
							list =>
							{
								foreach(Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 3))
								{
									if(
										state.Performer.AlliedWith(figure) &&
										figure is Character character &&
										character.Cards.Any(card => card.CardState == CardState.Hand && card.Top is HierophantPrayerCardSide))
									{
										list.Add(figure);
									}
								}
							}
						);

						if(figure == null)
						{
							return;
						}

						Character character = (Character)figure;
						AbilityCard prayerCard = await AbilityCmd.SelectAbilityCard(character,
							list =>
							{
								foreach(AbilityCard card in character.Cards)
								{
									if(card.CardState == CardState.Hand && card.Top is HierophantPrayerCardSide)
									{
										list.Add(card);
									}
								}
							}, CardState.Hand, false, hintText: "Select a prayer card to play for its top or bottom"
						);

						if(prayerCard == null)
						{
							return;
						}

						state.SetPerformed();

						AbilityCardSection section = await AbilityCmd.PerformAbilityCardTopOrBottom(figure, prayerCard);

						await AbilityCmd.GainXP(state.Performer, 1);

						if(section == AbilityCardSection.Bottom)
						{
							if(await AbilityCmd.AskConsumeElement(state.Performer, Element.Light,
								   $"Consume {Icons.Inline(Icons.GetElement(Element.Light))} to give a Prayer card?"))
							{
								await GivePrayerCard(state, figure);
							}
						}
					}
				)
				.Build())
		];
	}
}