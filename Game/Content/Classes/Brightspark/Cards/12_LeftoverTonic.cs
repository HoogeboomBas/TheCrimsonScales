using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class LeftoverTonic : BrightsparkCardModel<LeftoverTonic.CardTop, LeftoverTonic.CardBottom>
{
	public override string Name => "Leftover Tonic";
	public override int Level => 1;
	public override int Initiative => 70;
	protected override int AtlasIndex => 12;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer &&
						              parameters.Figure.TurnPerformedActionStates.SelectMany(actionState => actionState.AbilityStates)
							              .All(abilityState => abilityState is not AttackAbility.State || !abilityState.Performed),
						async parameters =>
						{
							ActionState actionState = new ActionState(parameters.Figure,
							[
								AttackAbility.Builder().WithDamage(2).WithRange(2).Build(),
							]);
							await actionState.Perform();
							await state.AdvanceUseSlot();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29050028f, 0.3929994f)),
					new UseSlot(new Vector2(0.49949962f, 0.3929994f)),
					new UseSlot(new Vector2(0.7080005f, 0.3929994f), GainXP)
				])
				.Build()),
		];

		public override bool Persistent => true;
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer &&
						              parameters.Figure.TurnPerformedActionStates.SelectMany(actionState => actionState.AbilityStates)
							              .All(abilityState => abilityState is not MoveAbility.State || !abilityState.Performed),
						async parameters =>
						{
							ActionState actionState = new ActionState(parameters.Figure,
							[
								MoveAbility.Builder().WithDistance(2).Build(),
							]);
							await actionState.Perform();
							await state.AdvanceUseSlot();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29150033f, 0.86599743f)),
					new UseSlot(new Vector2(0.49949986f, 0.86599743f)),
					new UseSlot(new Vector2(0.70699996f, 0.86599743f), GainXP)
				])
				.Build()),
		];

		public override bool Persistent => true;
	}
}