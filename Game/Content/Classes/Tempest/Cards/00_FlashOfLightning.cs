using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class FlashOfLightning : TempestCardModel<FlashOfLightning.CardTop, FlashOfLightning.CardBottom>
{
	public override string Name => "Flash of Lightning";
	public override int Level => 1;
	public override int Initiative => 11;
	protected override int AtlasIndex => 0;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6211789f, 0.14007422f)))
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Air,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(1);
							parameters.AbilityState.AdjustMoveType(MoveType.Jump);

							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Jump)}")))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.62163085f, 0.33432612f)))
				.WithDuringAttackSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(2, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);
						parameters.AbilityState.AbilityAddCondition(Conditions.Muddle);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.GetCondition(Conditions.Muddle))}")),

					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);

							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}"))
				])
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(1, new MoveCircle(this, new Vector2(0.623959f, 0.6171745f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Dodge)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DodgeUsedEvent.Subscribe(state, this,
						parameters => parameters.DodgeUser == state.Performer,
						async parameters =>
						{
							await AbilityCmd.InfuseElement(state, Element.Light);

							await AbilityCmd.AddCondition(state, parameters.DodgeUser, Conditions.Strengthen);

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DodgeUsedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.50056416f, 0.897507f), GainXP))
				.Build())
		];

		public override bool Persistent => true;
	}
}