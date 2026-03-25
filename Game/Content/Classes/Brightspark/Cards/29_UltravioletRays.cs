using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class UltravioletRays : BrightsparkCardModel<UltravioletRays.CardTop, UltravioletRays.CardBottom>
{
	public override string Name => "Ultraviolet Rays";
	public override int Level => 9;
	public override int Initiative => 66;
	protected override int AtlasIndex => 29;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
							parameters => parameters.AbilityState.IsSingleTarget,
							async applyParameters =>
							{
								applyParameters.AbilityState.SingleTargetAdjustAttackValue(2);
								await GDTask.CompletedTask;
							}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Attack)}")),
						checkDuplicates: false);
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
							parameters => parameters.AbilityState.IsSingleTarget,
							async applyParameters =>
							{
								applyParameters.AbilityState.SingleTargetAdjustPierce(3);
								await GDTask.CompletedTask;
							}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Pierce)}3")),
						checkDuplicates: false);
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
							parameters => parameters.AbilityState.IsSingleTarget,
							async applyParameters =>
							{
								applyParameters.AbilityState.SingleTargetAdjustPush(2);
								await GDTask.CompletedTask;
							}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Push)}2")), checkDuplicates: false);
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
							parameters => parameters.AbilityState.IsSingleTarget,
							async applyParameters =>
							{
								applyParameters.AbilityState.SingleTargetAddCondition(Conditions.Poison1);
								await GDTask.CompletedTask;
							}, effectInfoViewParameters: new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Poison1)))),
						checkDuplicates: false);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 3;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6203f, 0.631746f)))
				.WithDuringMovementSubscriptions(
					[
						ScenarioEvents.DuringMovement.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
							applyFunction: async parameters =>
							{
								parameters.AbilityState.AdjustMoveValue(3);

								await GDTask.CompletedTask;
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+3{Icons.Inline(Icons.Move)}")
						),
						ScenarioEvents.DuringMovement.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
							applyFunction: async parameters =>
							{
								parameters.AbilityState.AdjustMoveValue(3);

								await GDTask.CompletedTask;
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+3{Icons.Inline(Icons.Move)}")
						)
					]
				)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.InfuseWild(), CardElementInfusion.InfuseWild()];
	}
}