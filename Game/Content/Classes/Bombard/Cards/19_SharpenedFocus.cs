using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SharpenedFocus : BombardCardModel<SharpenedFocus.CardTop, SharpenedFocus.CardBottom>
{
	public override string Name => "Sharpened Focus";
	public override int Level => 5;
	public override int Initiative => 32;
	protected override int AtlasIndex => 19;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.ProjectileTokenCreatedEvent.Subscribe(state, this,
						parameters => parameters.TokenCreator == state.Performer,
						async parameters =>
						{
							await state.AdvanceUseSlot();
							ScenarioEvents.ProjectileTokenCreatedEvent.Unsubscribe(state, this);
							ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
								attackParameters =>
									attackParameters.Performer == state.Performer &&
									attackParameters.AbilityState.ActionState.ParentActionState != null &&
									attackParameters.AbilityState.ActionState.ParentActionState.AbilityStates.Any(parentAbilityState =>
										parentAbilityState is ProjectileAbility.State),
								async attackParameters =>
								{
									attackParameters.AbilityState.SingleTargetAdjustAttackValue(2);
									attackParameters.AbilityState.SingleTargetAdjustPierce(3);

									await state.AdvanceUseSlot();
								}
							);
							Figure figure = await AbilityCmd.SelectFigure(state,
								figures => figures.AddRange(parameters.Hex.GetHexObjectsOfType<Figure>()
									.Where(figure => figure.EnemiesWith(state.Performer))),
								hintText: () => $"Select an enemy to gain {Icons.HintText(Icons.GetCondition(Conditions.Immobilize))}");
							if(figure == null)
							{
								return;
							}

							await AbilityCmd.AddCondition(state, figure, Conditions.Immobilize);
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
					new UseSlot(new Vector2(0.3962963f, 0.36772484f)),
					new UseSlot(new Vector2(0.6059259f, 0.36772484f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(1, new MoveCircle(this, new Vector2(0.6237037f, 0.7112401f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer && parameters.AbilityState.SingleTargetRangeType == RangeType.Range,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustRange(1);
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Immobilize);
							parameters.AbilityState.SingleTargetSetHasAdvantage();
							ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);
							ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

							state.SetPerformed();
							await GDTask.CompletedTask;
						});

					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async _ =>
						{
							ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);
							ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.Build())
		];
	}
}