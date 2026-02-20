using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class VoraciousHunter : RuinmawCardModel<VoraciousHunter.CardTop, VoraciousHunter.CardBottom>
{
	public override string Name => "Voracious Hunter";
	public override int Level => 5;
	public override int Initiative => 32;
	protected override int AtlasIndex => 20;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					List<ScenarioEvents.AbilityEnded.Subscription> subscriptions = [];
					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure.EnemiesWith(state.Performer) &&
						                      canApplyParameters.PotentialAbilityState?.Performer == state.Performer,
						async applyParameters =>
						{
							bool hasPerformed = false;

							if(state.UseSlotIndex > 2)
							{
								return;
							}

							ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription sub = ScenarioEvents.AbilityEnded.Subscription.New(
								parameters => !hasPerformed,
								async parameters =>
								{
									hasPerformed = true;
									ScenarioEvents.AbilityEndedEvent.Unsubscribe(subscriptions);
									subscriptions.RemoveAt(0);
									ActionState actionState = new ActionState(state.Performer,
									[
										MoveAbility.Builder().WithDistance(4).Build(),
										AttackAbility.Builder().WithDamage(4).Build(),
									]);
									await actionState.Perform();
									ScenarioEvents.AbilityEndedEvent.Subscribe(state, new object(), subscriptions);
								}
							);
							subscriptions.Add(sub);
							ScenarioEvents.AbilityEndedEvent.Subscribe(state, new object(), sub);

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.28000042f, 0.28149942f), SateRuinmaw),
					new UseSlot(new Vector2(0.48500004f, 0.28149942f), GainXP),
					new UseSlot(new Vector2(0.690999f, 0.28149942f), GainXP)
				])
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.Build()),
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Self)
				.WithConditions(Ruinmaw.Empower)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustHealValue(2);
							parameters.AbilityState.AbilityAddCondition(Ruinmaw.Empower);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
		];
	}
}