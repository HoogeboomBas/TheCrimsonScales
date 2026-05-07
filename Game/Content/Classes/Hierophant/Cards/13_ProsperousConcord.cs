using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ProsperousConcord : HierophantCardModel<ProsperousConcord.CardTop, ProsperousConcord.CardBottom>
{
	public override string Name => "Prosperous Concord";
	public override int Level => 1;
	public override int Initiative => 43;
	protected override int AtlasIndex => 13 - 13;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder().WithDamage(2).WithRange(3).Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					Figure figure = state.GetCustomValue<Figure>(this, "Figure");

					await AbilityCmd.AddCharacterToken(state, figure, textParameters =>
						$"The next time an ally attacks this enemy this round, they add +2{Icons.Inline(Icons.Attack)} to the attack.");

					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters =>
							state.Performer.AlliedWith(canApplyParameters.Performer) &&
							canApplyParameters.AbilityState.Target == figure,
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAdjustAttackValue(2);

							await state.ActionState.RequestDiscardOrLose();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					Figure figure = state.GetCustomValue<Figure>(this, "Figure");

					await AbilityCmd.RemoveCharacterToken(state, figure);
				})
				.WithConditionalAbilityCheck(async state =>
				{
					if(!await AbilityCmd.HasPerformedAbility(state, 0))
					{
						return false;
					}

					Figure figure = await AbilityCmd.SelectFigure(state,
						list => list.AddRange(state.ActionState.GetAbilityState<AttackAbility.State>(0).UniqueTargetedFigures),
						hintText: () => "Place character token?");

					if(figure == null)
					{
						return false;
					}

					state.SetCustomValue(this, "Figure", figure);
					return true;
				})
				.WithSkipConfirmation()
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
		public override bool Round => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithRange(1,
					new RangeSquare(this, new Vector2(0.54945546f, 0.6825218f)),
					new RangeSquare(this, new Vector2(0.6705539f, 0.6825218f)))
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