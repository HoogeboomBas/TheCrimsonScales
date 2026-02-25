using System.Collections.Generic;
using System.Linq;

public class TrajectoryDiverter : ArtificerCardModel<TrajectoryDiverter.CardTop, TrajectoryDiverter.CardBottom>
{
	public override string Name => "Trajectory Diverter";
	public override int Level => 3;
	public override int Initiative => 33;
	protected override int AtlasIndex => 15;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(1)
				.WithConditions(Conditions.Disarm)
				.WithRange(3)
				.WithTrapCount(2)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					CreateTrapAbility.State trapState = state.ActionState.GetAbilityState<CreateTrapAbility.State>(0);

					ScenarioEvents.TrapTriggeredEvent.Subscribe(state, this,
						canApply: canApplyParameters => trapState.CreatedTraps.Contains(canApplyParameters.Trap),
						async _ =>
						{
							await GainScrapToken(state);
							await AbilityCmd.GainXP(state.Performer, 1);
						}
					);
					ScenarioEvents.HexObjectDestroyedEvent.Subscribe(state, this,
						_ => trapState.CreatedTraps.All(trap => trap.IsDestroyed),
						async _ =>
						{
							await state.ActionState.RequestDiscardOrLose();
						});
					foreach(Trap trap in trapState.CreatedTraps)
					{
						await AbilityCmd.AddCharacterToken(state, trap,
							$"Artificer gains 1{Icons.Inline(Artificer.ScrapToken)} when this trap is sprung.");
					}
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.TrapTriggeredEvent.Unsubscribe(state, this);
					ScenarioEvents.HexObjectDestroyedEvent.Unsubscribe(state, this);
					foreach(Trap trap in state.ActionState.GetAbilityState<CreateTrapAbility.State>(0).CreatedTraps)
					{
						await AbilityCmd.RemoveCharacterToken(state, trap);
					}
				})
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(3)
				.WithTarget(Target.Enemies | Target.Allies)
				.WithRange(4)
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.WithConditionalAbilityCheck(async state =>
					await LoseScrapTokensConditionalAbilityCheck(state.Performer, 1,
						new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Move)}3")))
				.Build()),
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(3)
				.WithTarget(Target.Enemies | Target.Allies)
				.WithRange(4)
				.Build())
		];
	}
}