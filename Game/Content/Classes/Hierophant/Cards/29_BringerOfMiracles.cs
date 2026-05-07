using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class BringerOfMiracles : HierophantLevelUpCardModel<BringerOfMiracles.CardTop, BringerOfMiracles.CardBottom>
{
	public override string Name => "Bringer of Miracles";
	public override int Level => 9;
	public override int Initiative => 36;
	protected override int AtlasIndex => 15 - 15;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithRange(3)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Light,
						applyFunction: async applyParameters =>
						{
							((HealAbility.State)applyParameters.AbilityState).AbilityAddCondition(Conditions.Strengthen);
							await AbilityCmd.GainXP(applyParameters.AbilityState.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Strengthen))}")
					)
				)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure healTarget = state.ActionState.GetAbilityState<HealAbility.State>(0).Target;
					//TODO: Add character token visual
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.AbilityState.Performer == healTarget && canApplyParameters.Type == AMDCardType.Null,
						async applyParameters =>
						{
							applyParameters.SetType(AMDCardType.Crit);

							await GDTask.CompletedTask;
						});

					ScenarioEvents.AfterAttackPerformedEvent.Subscribe(state, this,
						parameters => parameters.Performer == healTarget,
						async parameters =>
						{
							ActionState actionState = new(healTarget, [
								HealAbility.Builder()
									.WithHealValue(0)
									.WithTarget(Target.Self)
									.WithOnAbilityStarted(async state =>
									{
										state.AbilityAdjustHealValue(parameters.AbilityState.DamageDealt);
										await GDTask.CompletedTask;
									})
									.Build()
							]);
							await actionState.Perform();
							await state.ActionState.RequestDiscardOrLose();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, this);
					ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					for(int i = state.Performer.AMDCardDeck.DrawPile.Count - 1; i >= 0; i--)
					{
						if(state.Performer.AMDCardDeck.DrawPile[i].Model is BlessAMDCard)
						{
							state.Performer.AMDCardDeck.DrawPile[i].Drawn();
							state.Performer.AMDCardDeck.DrawPile.RemoveAt(i);
						}
					}

					await AbilityCmd.AddCondition(state, state.Performer, Conditions.Bless);
					//TODO: Make the bless card not removed on draw

					ScenarioEvents.InflictConditionEvent.Subscribe(state, this,
						parameters =>
							parameters.Target == state.Performer &&
							parameters.ConditionModel?.ImmunityCompareBaseConditions != null &&
							Conditions.Bless.ImmunityCompareBaseConditions != null &&
							parameters.ConditionModel.ImmunityCompareBaseConditions
								.Any(condition => Conditions.Bless.ImmunityCompareBaseConditions.Contains(condition)),
						async parameters =>
						{
							parameters.SetPrevented(true);

							await GDTask.CompletedTask;
						}
					);

					ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						parameters =>
						{
							parameters.AddImmunity(Conditions.Bless);
						}
					);

					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();
							await GDTask.CompletedTask;
						}
					);

					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						parameters => state.Performer.AMDCardDeck.DiscardPile.Any(card => card.Model is BlessAMDCard),
						async parameters =>
						{
							AMDCard bless = state.Performer.AMDCardDeck.DiscardPile.First(card => card.Model is BlessAMDCard);
							state.Performer.AMDCardDeck.DrawPile.Add(bless);
							state.Performer.AMDCardDeck.DiscardPile.Remove(bless);
							state.Performer.AMDCardDeck.ShuffleDrawPile();

							await GDTask.CompletedTask;
						}
					);
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Unsubscribe(state, this);
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.ImmunitiesVisualCheckEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
		public override int XP => 2;
		public override bool Persistent => true;
	}
}