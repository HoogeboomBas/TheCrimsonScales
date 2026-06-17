using System.Collections.Generic;
using Fractural.Tasks;

public class DarkenedOvercast : LuminaryCardModel<DarkenedOvercast.CardTop, DarkenedOvercast.CardBottom>
{
	public override string Name => "Darkened Overcast";
	public override int Level => 2;
	public override int Initiative => 10;
	protected override int AtlasIndex => 14;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Performer.EnemiesWith(state.Performer) &&
						                      RangeHelper.Distance(canApplyParameters.Performer.Hex, state.Performer.Hex) <= 3,
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetSetHasDisadvantage();

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Invisible)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						parameters => true,
						async parameters =>
						{
							await AbilityCmd.RemoveCondition(state.Performer, Conditions.Invisible, state);
						}
					);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithMandatory(true)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Dark)];
		public override bool Round => true;
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					int glowsPerformed = 0;
					ScenarioEvents.AbilityPerformedEvent.Subscribe(state, this,
						canApply: canApplyParameters => canApplyParameters.Performer == state.Performer &&
						                                canApplyParameters.AbilityState.GetCustomValue<bool>(state.Performer, "Glow Ability"),
						async applyParameters =>
						{
							glowsPerformed++;
							ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
							ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
							ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
								canApplyParameters =>
									canApplyParameters.Figure == state.Performer,
								applyParameters =>
								{
									applyParameters.AdjustShield(glowsPerformed);
								}
							);

							ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
								parameters => parameters.Figure == state.Performer && parameters.FromAttack,
								async parameters =>
								{
									parameters.AdjustShield(glowsPerformed);

									await GDTask.CompletedTask;
								}
							);

							await GDTask.CompletedTask;
						}
					);

					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						canApply: canApplyParameters => true,
						async applyParameters =>
						{
							glowsPerformed = 0;
							ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
							ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioEvents.AbilityPerformedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.InfuseWild()];
		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}