using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class UnendingTorment : ChainguardLevelUpCardModel<UnendingTorment.CardTop, UnendingTorment.CardBottom>
{
	public override string Name => "Unending Torment";
	public override int Level => 9;
	public override int Initiative => 33;
	protected override int AtlasIndex => 15 - 15;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.TrapTriggeredEvent.Subscribe(state, this,
						canApply: canApplyParameters => canApplyParameters.PotentialAbilityState?.Authority == state.Performer &&
						                                canApplyParameters.Figure.HasCondition(Chainguard.Shackle),
						async applyParameters =>
						{
							applyParameters.AdjustTrapDamage(applyParameters.Trap.Damage);

							await GDTask.CompletedTask;
						}
					);

					ScenarioEvents.InflictConditionEvent.Subscribe(state, this,
						canApply: parameters => parameters.ConditionModel is Shackle &&
						                        parameters.PotentialAbilityState != null &&
						                        parameters.PotentialAbilityState.Performer == state.Performer,
						async parameters =>
						{
							await AbilityCmd.SufferDamage(state, state.Performer, 1);
						},
						EffectType.MandatoryBeforeOptionals
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.TrapTriggeredEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		private MoveEnhancementMark _enhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_enhancementMark = new MoveCircle(this, new Vector2(0.6215662f, 0.82212216f));
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ControlAbility.Builder()
				.WithGetAbilities(state =>
					[
						MoveAbility.Builder()
							.WithDistance(3, _enhancementMark)
							.WithOnAbilityStarted(async moveState =>
							{
								ScenarioEvents.CanMoveFurtherCheckEvent.Subscribe(moveState, this,
									parameters =>
										parameters.Performer == state.Target &&
										moveState.Performer == parameters.Performer &&
										parameters.Performer.HasCondition(Chainguard.Shackle),
									async parameters =>
									{
										parameters.SetCanMoveFurther();

										await GDTask.CompletedTask;
									},
									// Go after shackle and unblock it
									order: 1
								);

								await GDTask.CompletedTask;
							})
							.WithAbilityStartedSubscription(
								ScenarioEvents.AbilityStarted.Subscription.New(
									parameters =>
										parameters.Performer == state.Target &&
										parameters.AbilityState is MoveAbility.State &&
										parameters.Performer.HasCondition(Chainguard.Shackle),
									parameters =>
									{
										parameters.SetIsBlocked(false);

										return GDTask.CompletedTask;
									},
									// Go after shackle and unblock it
									order: 1
								)
							)
							.WithOnAbilityEnded(async moveState =>
							{
								ScenarioEvents.CanMoveFurtherCheckEvent.Unsubscribe(moveState, this);

								await GDTask.CompletedTask;
							})
							.Build()
					]
				)
				.WithCustomGetTargets((state, figures) =>
				{
					IEnumerable<Figure> adjacentFigures = RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, includeOrigin: false);
					figures.AddRange(adjacentFigures.Where(figure => figure.EnemiesWith(state.Performer) && figure.HasCondition(Chainguard.Shackle)));
				})
				.WithTarget(Target.Enemies)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AfterAttackPerformedEvent.Subscribe(state, this,
						canApply: parameters => parameters.AbilityState.Target.HasCondition(Chainguard.Shackle) &&
						                        parameters.AbilityState.Target.EnemiesWith(state.Performer),
						async parameters =>
						{
							await AbilityCmd.SufferDamage(state, parameters.AbilityState.Target, 1);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}
}