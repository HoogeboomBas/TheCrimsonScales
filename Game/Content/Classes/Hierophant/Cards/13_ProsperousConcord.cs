using System.Collections.Generic;
using Fractural.Tasks;

public class ProsperousConcord : HierophantCardModel<ProsperousConcord.CardTop, ProsperousConcord.CardBottom>
{
	public override string Name => "Prosperous Concord";
	public override int Level => 1;
	public override int Initiative => 43;
	protected override int AtlasIndex => 13 - 13;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder().WithDamage(2).WithRange(3).Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					//TODO: Add visual (character token) to target(?)
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters =>
							state.Performer.AlliedWith(canApplyParameters.Performer) &&
							canApplyParameters.AbilityState.Target == attackAbilityState.UniqueTargetedFigures[0],
						async applyParameters =>
						{
							//TODO: Add visual (character token) to target(?)
							applyParameters.AbilityState.SingleTargetAdjustAttackValue(2);

							await state.ActionState.RequestDiscardOrLose();

							//await AbilityCmd.DiscardOrLose(AbilityCard);
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Light];
		protected override bool Round => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithRange(1)
				.WithAfterTargetConfirmedSubscriptions(
					[
						ScenarioEvents.HealAfterTargetConfirmed.Subscription.New(
							canApplyFunction: canApplyParameters =>
								canApplyParameters.AbilityState.Performer.AlliedWith(canApplyParameters.AbilityState.Target),
							applyFunction: async applyParameters =>
							{
								applyParameters.AbilityState.SetCustomValue(this, "StrengthenAdded", true);

								applyParameters.AbilityState.SingleTargetAddCondition(Conditions.Strengthen);
								applyParameters.AbilityState.SingleTargetAdjustHealValue(-2);

								await GDTask.CompletedTask;
							},
							effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Strengthen)),
							effectInfoViewParameters: new TextEffectInfoView.Parameters(
								$"-2{Icons.Inline(Icons.Heal)}, {Icons.Inline(Icons.GetCondition(Conditions.Strengthen))}"),
							effectType: EffectType.Selectable
						),

						ScenarioEvents.HealAfterTargetConfirmed.Subscription.ConsumeElement(Element.Light,
							canApplyFunction: canApplyParameters => canApplyParameters.AbilityState.GetCustomValue<bool>(this, "StrengthenAdded"),
							applyFunction: async applyParameters =>
							{
								applyParameters.AbilityState.SingleTargetAdjustHealValue(2);

								await GDTask.CompletedTask;
							},
							effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Heal)}")
						)
					]
				)
				.Build())
		];
	}
}