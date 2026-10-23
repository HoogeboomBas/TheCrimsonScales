using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class RagingFire : RimehearthCardModel<RagingFire.CardTop, RagingFire.CardBottom>
{
	public override string Name => "Raging Fire";
	public override int Level => 7;
	public override int Initiative => 36;
	protected override int AtlasIndex => 24;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Wound1)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(2);

							await state.AdvanceUseSlot();
						}, canApplyMultipleTimesDuringSubscription: false);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.29024962f, 0.3390582f), GainXP),
					new UseSlot(new Vector2(0.49901202f, 0.3390582f)),
					new UseSlot(new Vector2(0.7069984f, 0.3390582f), GainXP)
				])
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62115484f, 0.6229687f)))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithTarget(Target.Self)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AbilityAdjustHealValue(3);
							applyParameters.AbilityState.SetCustomValue(this, "FireConsumed", true);

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+3{Icons.Inline(Icons.Heal)}")
					))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.AddCondition(state, state.Performer, Conditions.Wound1);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return state.ActionState.GetAbilityState<HealAbility.State>(1).GetCustomValue<bool>(this, "FireConsumed");
				})
				.Build())
		];
	}
}