using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SolidLight : LuminaryCardModel<SolidLight.CardTop, SolidLight.CardBottom>
{
	public override string Name => "Solid Light";
	public override int Level => 1;
	public override int Initiative => 12;
	protected override int AtlasIndex => 11;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1, new ShieldDiamondPlus(this, new Vector2(0.6196841f, 0.16519174f)))
				.Build()),

			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Ice))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AfterAttackPerformedEvent.Subscribe(state, this,
						canApply: parameters => parameters.AbilityState.Target == state.Performer,
						apply: async parameters =>
						{
							await AbilityCmd.InfuseElement(state, Element.Light);
							ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override bool Round => true;
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62128437f, 0.6504817f)))
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(1);
							parameters.AbilityState.AdjustMoveType(MoveType.Jump);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Move)}, {Icons.Inline(Icons.Jump)}")
					)
				)
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithRange(2)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Ice))
				.Build())
		];
	}
}