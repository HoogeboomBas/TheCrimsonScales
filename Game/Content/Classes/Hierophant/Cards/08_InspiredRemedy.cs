using System.Collections.Generic;
using Fractural.Tasks;

public class InspiredRemedy : HierophantCardModel<InspiredRemedy.CardTop, InspiredRemedy.CardBottom>
{
	public override string Name => "Inspired Remedy";
	public override int Level => 1;
	public override int Initiative => 76;
	protected override int AtlasIndex => 13 - 8;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithRange(2)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Light,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AbilityAdjustHealValue(1);
							applyParameters.AbilityState.AbilityAdjustRange(1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Heal)}, +1{Icons.Inline(Icons.Range)}"))
				)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.HealAfterTargetConfirmed.Subscription.New(
						applyFunction: async applyParameters =>
						{
							bool underHalfHP = applyParameters.AbilityState.Target.Health <= applyParameters.AbilityState.Target.MaxHealth / 2;
							applyParameters.AbilityState.SetCustomValue(this, "UnderHalfHP", underHalfHP);

							await GDTask.CompletedTask;
						})
				)
				.WithAfterHealPerformedSubscription(
					ScenarioEvents.AfterHealPerformed.Subscription.New(
						canApplyFunction: canApplyParameters => 
							canApplyParameters.Performer.AlliedWith(canApplyParameters.AbilityState.Target) && 
							canApplyParameters.AbilityState.Target is Character &&
							canApplyParameters.AbilityState.GetCustomValue<bool>(this, "UnderHalfHP"),
						applyFunction: async applyParameters =>
						{
							await GivePrayerCard(applyParameters.AbilityState, applyParameters.AbilityState.Target);
						}
					)
				)
				.Build())
		];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure == state.Performer,
						async applyParameters =>
						{
							ActionState actionState = new ActionState(state.Performer, [
								HealAbility.Builder()
									.WithHealValue(1)
									.WithRange(1)
									.WithTarget(Target.Allies)
									.Build()
							]);
							await actionState.Perform();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}