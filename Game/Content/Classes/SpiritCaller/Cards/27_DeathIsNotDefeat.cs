using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class DeathIsNotDefeat : SpiritCallerCardModel<DeathIsNotDefeat.CardTop, DeathIsNotDefeat.CardBottom>
{
	public override string Name => "Death is not Defeat";
	public override int Level => 9;
	public override int Initiative => 97;
	protected override int AtlasIndex => 28 - 27;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.BeforeFigureKilledEvent.Subscribe(state, this,
						parameters =>
							state.Performer.AlliedWith(parameters.Figure) &&
							parameters.Figure is Character character &&
							character.Cards.Count(card => card.CardState is CardState.Hand or CardState.Discarded) < 2,
						async parameters =>
						{
							parameters.SetPrevented();

							List<AbilityCard> selectedAbilityCards =
								await AbilityCmd.SelectAbilityCards(state.Performer as Character, CardState.Lost, 0, 4,
									hintText: "Select up to four lost cards to recover");

							foreach(AbilityCard selectedAbilityCard in selectedAbilityCards)
							{
								await AbilityCmd.ReturnToHand(selectedAbilityCard);
							}

							ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
								turnStartedParameters =>
									turnStartedParameters.Figure == parameters.Figure,
								async turnStartedParameters =>
								{
									await AbilityCmd.AddCondition(state, parameters.Figure, Conditions.Invisible);
								}
							);

							ScenarioCheckEvents.CountsAsSpiritCheckEvent.Subscribe(state, this,
								countsAsSpiritParameters => countsAsSpiritParameters.Figure == parameters.Figure,
								countsAsSpiritParameters =>
								{
									countsAsSpiritParameters.SetCountsAsSpirit();
								}
							);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.BeforeFigureKilledEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.CountsAsSpiritCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Unrecoverable => true;
		public override bool Loss => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder()
						.WithDistance(4)
						.Build()
				])
				.WithCustomGetTargets((state, list) =>
				{
					if(state.GetCustomValue<bool>(this, "ConsumedElements"))
					{
						foreach(Figure figure in GameController.Instance.Map.Figures)
						{
							if(state.Performer.AlliedWith(figure))
							{
								list.Add(figure);
							}
						}
					}
					else
					{
						list.AddRange(Spirit.GetAllSpirits());
					}

					list.Add(state.Performer);
				})
				.WithTarget(Target.Any | Target.TargetAll)
				.WithCanTargetNonFigures()
				.WithAbilityStartedSubscription(ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(
					[CardElementConsumption.Consume(Element.Air), CardElementConsumption.Consume(Element.Dark)],
					applyFunction: async parameters =>
					{
						parameters.AbilityState.SetCustomValue(this, "ConsumedElements", true);

						await GainXP(parameters.AbilityState);
					},
					effectInfoViewParameters: new TextEffectInfoView.Parameters(
						$"Grant the move to all allies and self instead."))
				)
				.Build())
		];
	}
}