using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class DizzyingRelease : ChainguardLevelUpCardModel<DizzyingRelease.CardTop, DizzyingRelease.CardBottom>
{
	public override string Name => "Dizzying Release";
	public override int Level => 4;
	public override int Initiative => 24;
	protected override int AtlasIndex => 15 - 4;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Figure figure = await AbilityCmd.SelectFigure(state,
						list =>
						{
							list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, includeOrigin: false)
								.Where(figure => figure.EnemiesWith(state.Performer) && figure.HasCondition(Chainguard.Shackle)));
						}, hintText: () => $"Designate an adjacent enemy with {Icons.Inline(Icons.GetCondition(Chainguard.Shackle))}");

					if(figure != null)
					{
						state.SetCustomValue(this, "DesignatedEnemy", figure);
						state.SetPerformed();
					}
				})
				.Build()),

			new AbilityCardAbility(SwingAbility.Builder()
				.WithSwing(6)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.Add(state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<Figure>(this, "DesignatedEnemy"));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build()),

			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(3)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.Add(state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<Figure>(this, "DesignatedEnemy"));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build()),

			new AbilityCardAbility(SwingAbility.Builder()
				.WithSwing(0)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.Add(state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<Figure>(this, "DesignatedEnemy"));
				})
				.WithOnAbilityStarted(async state =>
				{
					if(!await AbilityCmd.HasPerformedAbility(state, 0))
					{
						return;
					}

					SwingAbility.State swingAbilityState = state.ActionState.GetAbilityState<SwingAbility.State>(1);
					int remainingSwing = swingAbilityState.AbilitySwing - swingAbilityState.SingleTargetState.ForcedMovementHexes.Count;
					state.AbilityAdjustSwing(remainingSwing);

					if(swingAbilityState.SingleTargetState.ForcedMovementHexes.Count > 0)
					{
						ScenarioEvents.SwingDirectionCheckEvent.Subscribe(state, this,
							canApply: parameters => state == parameters.AbilityState,
							apply: async parameters =>
							{
								bool clockwise = MoveHelper.IsClockwise(state.Performer.Hex, swingAbilityState.TargetedHexes[0],
									swingAbilityState.SingleTargetState.ForcedMovementHexes[0]);
								parameters.SetRequiredSwingDirection(clockwise ? SwingDirectionType.Clockwise : SwingDirectionType.Counterclockwise);

								ScenarioEvents.SwingDirectionCheckEvent.Unsubscribe(state, this);

								await GDTask.CompletedTask;
							}
						);
					}

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(async state =>
				{
					if(!await AbilityCmd.HasPerformedAbility(state, 0))
					{
						return false;
					}

					SwingAbility.State swingAbilityState = state.ActionState.GetAbilityState<SwingAbility.State>(1);
					int remainingSwing = swingAbilityState.AbilitySwing - swingAbilityState.SingleTargetState.ForcedMovementHexes.Count;

					return remainingSwing > 0;
				})
				.WithOnAbilityEnded(async state =>
				{
					ScenarioEvents.SwingDirectionCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(0)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.Add(state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<Figure>(this, "DesignatedEnemy"));
				})
				.WithDuringAttackSubscription(ScenarioEvents.DuringAttack.Subscription.New(
					applyFunction: async parameters =>
					{
						SwingAbility.State firstState = parameters.AbilityState.ActionState.GetAbilityState<SwingAbility.State>(1);
						PushAbility.State secondState = parameters.AbilityState.ActionState.GetAbilityState<PushAbility.State>(2);
						SwingAbility.State thirdState = parameters.AbilityState.ActionState.GetAbilityState<SwingAbility.State>(3);
						parameters.AbilityState.AbilityAdjustAttackValue(firstState.SingleTargetState.ForcedMovementHexes.Count +
						                                                  secondState.SingleTargetState.ForcedMovementHexes.Count +
						                                                  thirdState.SingleTargetState.ForcedMovementHexes.Count);

						await GDTask.CompletedTask;
					}))
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build()),
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Wound1)
				.WithCustomAsset("res://Content/Classes/Chainguard/Traps/ChainguardWoodSpikeTrap.tscn")
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					CreateTrapAbility.State createTrapState = state.ActionState.GetAbilityState<CreateTrapAbility.State>(0);

					ScenarioEvents.TrapTriggeredEvent.Subscribe(state, this,
						canApply: canApplyParameters => createTrapState.CreatedTraps.Contains(canApplyParameters.Trap),
						async applyParameters =>
						{
							ScenarioEvents.TrapTriggeredEvent.Unsubscribe(state, this);
							if(applyParameters.Figure.HasCondition(Chainguard.Shackle))
							{
								await AbilityCmd.SufferDamage(state, applyParameters.Figure, 1);
								await AbilityCmd.RemoveCondition(applyParameters.Figure, Chainguard.Shackle, state);
							}

							await state.ActionState.RequestDiscardOrLose();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.TrapTriggeredEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}
}