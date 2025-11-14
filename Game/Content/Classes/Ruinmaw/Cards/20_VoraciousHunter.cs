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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					List<ScenarioEvents.AbilityEnded.Subscription> subscriptions = [];
					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure.EnemiesWith(state.Performer) && canApplyParameters.PotentialAbilityState?.Performer == state.Performer,
						async applyParameters =>
						{
							if(state.UseSlotIndex == 0)
							{
								await SateRuinmaw(state.Performer);
							}
							if (state.UseSlotIndex > 2)
							{
								_removeImmediately = true;
								await state.ActionState.RequestDiscardOrLose();
								return;
                            }
							await state.AdvanceUseSlot();
							bool hasPerformed = false;

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

							await GDTask.CompletedTask;
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
					new UseSlot(new Vector2(0.16650043f, 0.3549993f)),
					new UseSlot(new Vector2(0.36999783f, 0.3549993f), GainXP),
					new UseSlot(new Vector2(0.78700954f, 0.3549993f), GainXP)
				])
				//TODO: Fix use slot positioning
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.Build()),
		];

		private bool _removeImmediately = false;
		protected override bool Persistent => !_removeImmediately;
		protected override bool RemoveImmediately => _removeImmediately;
		protected override bool Loss => true;
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Self)
				.WithConditions(Conditions.EmpowerRuinmaw)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustHealValue(2);
							parameters.AbilityState.AbilityAddCondition(Conditions.EmpowerRuinmaw);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
		];
	}
}