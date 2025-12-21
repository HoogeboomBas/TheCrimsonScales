using System.Collections.Generic;
using Fractural.Tasks;

public class Flashover : FireKnightLevelUpCardModel<Flashover.CardTop, Flashover.CardBottom>
{
	public override string Name => "Flashover";
	public override int Level => 9;
	public override int Initiative => 96;
	protected override int AtlasIndex => 1;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					SummonAbility summonAbility = SummonDrakefiend();
					await summonAbility.Perform(state.ActionState);
					SummonAbility.State summonAbilityState = state.ActionState.GetAbilityState<SummonAbility.State>(1);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(summonAbilityState, summonAbility);
					int characterTokens = 0;

					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure == summonAbilityState.Summon,
						async parameters =>
						{
							if (characterTokens < 2)
							{
								characterTokens++;
								//TODO: Add visual indicator for number of character tokens
								ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
									canApplyParameters => canApplyParameters.Figure == state.Performer,
									async applyParameters =>
									{
										summonAbility = SummonDrakefiend();
										await summonAbility.Perform(state.ActionState);
										summonAbilityState = state.ActionState.GetAbilityState<SummonAbility.State>(characterTokens+1);
										ScenarioEvents.FigureKilledEvent.Unsubscribe(summonAbilityState, summonAbility);
										ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);

										await GDTask.CompletedTask;
									}
								);
							}
							else
							{
								await state.ActionState.RequestDiscardOrLose();
							}

							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;

		private SummonAbility SummonDrakefiend()
        {
			return SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 2,
					Move = 3,
					Attack = 2,
					Range = 2,
					Traits = [new FlyingTrait(), new InfuseElementAfterAttackTrait(Element.Fire)]
				})
				.WithName("Reigniting Drakefiend")
				.WithTexturePath("res://Content/Classes/FireKnight/Drakefiend.jpg")
				.Build();
        }
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.WithOnAbilityStarted(async abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Subscribe(abilityState, this,
						canApplyParameters =>
							canApplyParameters.AbilityState == abilityState &&
							(canApplyParameters.Hex.HasHexObjectOfType<DifficultTerrain>() ||
							 canApplyParameters.Hex.HasHexObjectOfType<HazardousTerrain>()),
						applyParameters =>
						{
							if(applyParameters.Hex.HasHexObjectOfType<DifficultTerrain>())
							{
								applyParameters.SetMoveCost(1);
							}

							if(applyParameters.Hex.HasHexObjectOfType<HazardousTerrain>())
							{
								applyParameters.SetAffectedByNegativeHex(false);
							}
						}
					);

					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(abilityState, this,
						canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == abilityState.Performer,
						async applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							await GDTask.CompletedTask;
						}
					);

					if(abilityState.Performer.Hex.HasHexObjectOfType<Ladder>())
					{
						abilityState.AdjustMoveValue(1);
						abilityState.AddJump();
					}

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async abilityState =>
					{
						ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(abilityState, this);
						ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);

						await GDTask.CompletedTask;
					}
				)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					foreach(Figure target in RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, false))
					{
						await AbilityCmd.SufferDamage(null, target, 2);
						if (state.GetCustomValue<bool>(this, "Fire Consumed"))
						{
							await AbilityCmd.AddCondition(state, target, Conditions.Wound1);
						}
						state.SetPerformed();
					}
					
					await GDTask.CompletedTask;
				})
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.SetCustomValue(this, "Fire Consumed", true);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"Add {Icons.Inline(Icons.GetCondition(Conditions.Wound1))}")
					)
				)
				.WithConditionalAbilityCheck(async state =>
                {
                    ConfirmPrompt.Answer confirmAnswer =
						await PromptManager.Prompt(new ConfirmPrompt(null, () => "Perform damage ability?"), state.Authority);
					
					return confirmAnswer.Confirmed;
                })
				.Build())
		];
	}
}