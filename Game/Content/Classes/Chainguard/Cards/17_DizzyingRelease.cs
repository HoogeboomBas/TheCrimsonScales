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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SwingAbility.Builder()
				.WithSwing(6)
				.WithCustomGetTargets((state, figures) =>
				{
					IEnumerable<Figure> adjacentFigures = RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, includeOrigin: false);
					figures.AddRange(adjacentFigures.Where(figure => figure.EnemiesWith(state.Performer) && figure.HasCondition(Chainguard.Shackle)));
				})
				.Build()),

			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(3)
				.WithCustomGetTargets((state, figures) =>
				{
					SwingAbility.State swingAbilityState = state.ActionState.GetAbilityState<SwingAbility.State>(0);
					figures.AddRange(swingAbilityState.UniqueTargetedFigures);
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<SwingAbility.State>(0).Performed;
				})
				.Build()),

			new AbilityCardAbility(SwingAbility.Builder()
				.WithSwing(0)
				.WithCustomGetTargets((state, figures) =>
				{
					SwingAbility.State swingAbilityState = state.ActionState.GetAbilityState<SwingAbility.State>(0);
					figures.AddRange(swingAbilityState.UniqueTargetedFigures);
				})
				.WithOnAbilityStarted(async state =>
				{
					SwingAbility.State swingAbilityState = state.ActionState.GetAbilityState<SwingAbility.State>(0);
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
					SwingAbility.State swingAbilityState = state.ActionState.GetAbilityState<SwingAbility.State>(0);
					int remainingSwing = swingAbilityState.AbilitySwing - swingAbilityState.SingleTargetState.ForcedMovementHexes.Count;

					await GDTask.CompletedTask;

					return swingAbilityState.Performed && remainingSwing > 0;
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
					SwingAbility.State swingAbilityState = state.ActionState.GetAbilityState<SwingAbility.State>(0);
					figures.AddRange(swingAbilityState.UniqueTargetedFigures);
				})
				.WithOnAbilityStarted(async state =>
				{
					SwingAbility.State firstState = state.ActionState.GetAbilityState<SwingAbility.State>(0);
					PushAbility.State secondState = state.ActionState.GetAbilityState<PushAbility.State>(1);
					SwingAbility.State thirdState = state.ActionState.GetAbilityState<SwingAbility.State>(2);
					state.AbilityAdjustAttackValue(firstState.SingleTargetState.ForcedMovementHexes.Count +
					                               secondState.SingleTargetState.ForcedMovementHexes.Count +
					                               thirdState.SingleTargetState.ForcedMovementHexes.Count);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<SwingAbility.State>(0).Performed;
				})
				.Build()),
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
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
								await AbilityCmd.SufferDamage(null, applyParameters.Figure, 1);
								await AbilityCmd.RemoveCondition(applyParameters.Figure, Chainguard.Shackle);
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
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<CreateTrapAbility.State>(0).Performed;
				})
				.Build())
		];

		protected override int XP => 1;
		protected override bool Persistent => true;
	}
}