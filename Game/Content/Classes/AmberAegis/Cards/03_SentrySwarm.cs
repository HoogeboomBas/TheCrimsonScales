using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SentrySwarm : AmberAegisCardModel<SentrySwarm.CardTop, SentrySwarm.CardBottom>
{
	public override string Name => "Sentry Swarm";
	public override int Level => 1;
	public override int Initiative => 07;
	protected override int AtlasIndex => 3;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Subscribe(state, this,
						parameters => parameters.RetaliatingFigure == state.Performer &&
						              parameters.AbilityState.SingleTargetRangeType == RangeType.Range &&
						              RangeHelper.Distance(state.Performer.Hex, parameters.Performer.Hex) <= 4,
						async parameters =>
						{
							parameters.AdjustRetaliate(2);
							await state.AdvanceUseSlot();
						});
					
					ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Figure == state.Performer,
						applyParameters =>
						{
							applyParameters.AddRetaliate(2, 4);
						}
					);
					
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29142937f, 0.2973545f), GainXP),
					new UseSlot(new Vector2(0.49851853f, 0.2973545f)),
					new UseSlot(new Vector2(0.7074074f, 0.2973545f), GainXP),
					new UseSlot(new Vector2(0.18740742f, 0.4269841f)),
					new UseSlot(new Vector2(0.39555556f, 0.4269841f), GainXP),
					new UseSlot(new Vector2(0.60444444f, 0.4269841f)),
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.AbilityState.Target == state.Performer && !state.Performer.HasCondition(Conditions.Ward),
						async parameters =>
						{
							await AbilityCmd.AddCondition(state, parameters.AbilityState.Target, Conditions.Ward);
							await state.AdvanceUseSlot();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.3962963f, 0.8375661f)),
					new UseSlot(new Vector2(0.60518515f, 0.8375661f), GainXP)
				])
				.Build())
		];

		public override bool Round => true;
	}
}