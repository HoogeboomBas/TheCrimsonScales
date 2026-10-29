using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BulwarkOfEther : RimehearthCardModel<BulwarkOfEther.CardTop, BulwarkOfEther.CardBottom>
{
	public override string Name => "Bulwark of Ether";
	public override int Level => 5;
	public override int Initiative => 15;
	protected override int AtlasIndex => 19;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithTarget(Target.Self)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AbilityAdjustHealValue(2);
							applyParameters.AbilityState.AbilityAddCondition(Conditions.Strengthen);

							await AbilityCmd.InfuseElement(applyParameters.AbilityState, Element.Ice);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+2{Icons.Inline(Icons.Heal)}, {Icons.Inline(Icons.GetCondition(Conditions.Strengthen))}, {Icons.Inline(Icons.GetElement(Element.Ice))}")
					))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditionalAbilityCheck(state =>
					AbilityCmd.AskConsumeElement(state.Performer, Element.Ice, effectInfoText: $"{Icons.Inline(Icons.Attack)}3"))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.InfuseElement(state, Element.Fire);
				})
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters =>
							parameters.Figure == state.Performer && parameters.FromAttack && parameters.WouldSufferDamage,
						async parameters =>
						{
							parameters.AdjustShield(3);

							object subscriber = new object();

							ScenarioEvents.RetaliateEvent.Subscribe(state, this,
								canApplyParameters => canApplyParameters.RetaliatingFigure == state.Performer &&
								                      RangeHelper.Distance(canApplyParameters.AbilityState.Performer.Hex, state.Performer.Hex) <= 1 &&
								                      canApplyParameters.AbilityState == parameters.PotentialAbilityState,
								async applyParameters =>
								{
									applyParameters.AdjustRetaliate(3);

									await GDTask.CompletedTask;
								}
							);

							ScenarioEvents.AfterAttackPerformedEvent.Subscribe(state, subscriber,
								canApplyParameters => canApplyParameters.AbilityState == parameters.PotentialAbilityState,
								async _ =>
								{
									ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
									ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(state, subscriber);

									await GDTask.CompletedTask;
								}
							);

							await AbilityCmd.InfuseElement(state, [Element.Fire, Element.Ice]);

							await state.AdvanceUseSlot();
						}
					);

					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Figure == state.Performer,
						applyParameters =>
						{
							applyParameters.AdjustShield(3);
						}
					);

					ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Figure == state.Performer,
						applyParameters =>
						{
							applyParameters.AddRetaliate(3, 1);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.16417438f, 0.80564827f), GainXP),
					new UseSlot(new Vector2(0.3717368f, 0.80564827f)),
					new UseSlot(new Vector2(0.57894707f, 0.80564827f)),
					new UseSlot(new Vector2(0.78926164f, 0.80564827f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}