using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class ChampionOfChains : ChainguardLevelUpCardModel<ChampionOfChains.CardTop, ChampionOfChains.CardBottom>
{
	public override string Name => "Champion of Chains";
	public override int Level => 9;
	public override int Initiative => 10;
	protected override int AtlasIndex => 15 - 14;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Subscribe(state, this,
						canApply: parameters =>
							parameters.ConditionModel is Shackle &&
							parameters.PotentialAbilityState != null &&
							parameters.PotentialAbilityState.Performer == state.Performer,
						async parameters =>
						{
							await AbilityCmd.AddCondition(state, parameters.Target, Conditions.Wound1);
						}
					);

					ScenarioCheckEvents.MaxShackleCountCheckEvent.Subscribe(state, this,
						parameters => parameters.Shackler == state.Performer,
						parameters =>
						{
							parameters.AdjustMaxShackleCount(2);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.MaxShackleCountCheckEvent.Unsubscribe(state, this);

					int maxShackleCount = Chainguard.GetMaxShackleCount(state.Performer);
					await Chainguard.RemoveAllExtraShackles(state.Performer, maxShackleCount);
				})
				.Build()),

			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(2)
				.WithRange(3)
				.WithConditions(Chainguard.Shackle)
				.WithTargets(3)
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Figure figure = await AbilityCmd.SelectFigure(state,
						list =>
						{
							list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 3, includeOrigin: false)
								.Where(figure => figure.EnemiesWith(state.Performer)));
						}, hintText: () => $"Designate an enemy within {Icons.Inline(Icons.Range)}3.");

					if(figure != null)
					{
						await AbilityCmd.AddCondition(state, figure, Chainguard.Shackle);
						state.SetCustomValue(this, "DesignatedEnemy", figure);
						state.SetPerformed();
					}
				})
				.Build()),

			new AbilityCardAbility(SwingAbility.Builder()
				.WithSwing(6)
				.WithRange(3)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.Add(state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<Figure>(this, "DesignatedEnemy"));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build()),

			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(4)
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

					SwingAbility.State swingState = state.ActionState.GetAbilityState<SwingAbility.State>(1);

					int remainingSwing = swingState.Performed ? swingState.AbilitySwing - swingState.SingleTargetState.ForcedMovementHexes.Count : swingState.AbilitySwing;

					state.AbilityAdjustSwing(remainingSwing);

					if(swingState.Performed && swingState.SingleTargetState.ForcedMovementHexes.Count > 0)
					{
						ScenarioEvents.SwingDirectionCheckEvent.Subscribe(state, this,
							canApply: parameters => state == parameters.AbilityState,
							apply: async parameters =>
							{
								bool clockwise = MoveHelper.IsClockwise(state.Performer.Hex, swingState.TargetedHexes[0],
									swingState.SingleTargetState.ForcedMovementHexes[0]);
								parameters.SetRequiredSwingDirection(clockwise ? SwingDirectionType.Clockwise : SwingDirectionType.Counterclockwise);

								ScenarioEvents.SwingDirectionCheckEvent.Unsubscribe(state, this);

								await GDTask.CompletedTask;
							}
						);
					}
				})
				.WithConditionalAbilityCheck(async state =>
				{
					if(!await AbilityCmd.HasPerformedAbility(state, 0))
					{
						return false;
					}

					SwingAbility.State swingState = state.ActionState.GetAbilityState<SwingAbility.State>(1);

					int remainingSwing = swingState.Performed ? swingState.AbilitySwing - swingState.SingleTargetState.ForcedMovementHexes.Count : swingState.AbilitySwing;

					return remainingSwing > 0;
				})
				.WithOnAbilityEnded(async state =>
				{
					ScenarioEvents.SwingDirectionCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
		];
	}
}