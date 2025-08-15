using System.Collections.Generic;
using Fractural.Tasks;

public class ProsperousConcord : HierophantCardModel<ProsperousConcord.CardTop, ProsperousConcord.CardBottom>
{
	public override string Name => "Prosperous Concord";
	public override int Level => 1;
	public override int Initiative => 43;
	protected override int AtlasIndex => 29 - 11;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(2, range: 3)),

			new AbilityCardAbility(new OtherActiveAbility(
				async state =>
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
				},
				async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				},
				conditionalAbilityCheck: state => AbilityCmd.HasPerformedAbility(state, 0)
			))
		];

		protected override IEnumerable<Element> Elements => [Element.Light];
		protected override bool Round => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new HealAbility(3, range: 1,
				afterTargetConfirmedSubscriptions:
				[
					ScenarioEvent<ScenarioEvents.HealAfterTargetConfirmed.Parameters>.Subscription.New(
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
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"-2{Icons.Inline(Icons.Heal)}, {Icons.Inline(Icons.GetCondition(Conditions.Strengthen))}"),
						effectType: EffectType.Selectable
					),

					ScenarioEvent<ScenarioEvents.HealAfterTargetConfirmed.Parameters>.Subscription.ConsumeElement(Element.Light,
						canApplyFunction: canApplyParameters => canApplyParameters.AbilityState.GetCustomValue<bool>(this, "StrengthenAdded"),
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAdjustHealValue(2);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Heal)}")
					)
				]
			))
		];
	}
}