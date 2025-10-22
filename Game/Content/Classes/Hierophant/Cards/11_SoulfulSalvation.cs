using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SoulfulSalvation : HierophantCardModel<SoulfulSalvation.CardTop, SoulfulSalvation.CardBottom>
{
	public override string Name => "Soulful Salvation";
	public override int Level => 1;
	public override int Initiative => 11;
	protected override int AtlasIndex => 13 - 11;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						canApplyParameters => state.Authority.EnemiesWith(canApplyParameters.Figure),
						async applyParameters =>
						{
							ActionState actionState = new ActionState(state.Performer,
							[
								HealAbility.Builder()
									.WithHealValue(2)
									.WithConditions(Conditions.Bless)
									.WithTarget(Target.Allies | Target.TargetAll)
									.WithCustomGetTargets((healAbilityState, list) =>
									{
										list.AddRange(RangeHelper.GetFiguresInRange(applyParameters.Figure.Hex, 1));
									})
									.Build()
							]);
							await actionState.Perform();

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.3f)))
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Light];
		protected override bool Persistent => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.JustBeforeSufferDamageEvent.Subscribe(state, this,
						parameters =>
							parameters.Figure is Character &&
							state.Performer.AlliedWith(parameters.Figure) &&
							!parameters.Prevented &&
							parameters.Figure.Health <= parameters.Damage,
						async parameters =>
						{
							parameters.SetPrevented();

							ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription recoverCardSubscription =
								ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
									subscriptionParameters => true,
									async subscriptionParameters =>
									{
										Character character = (Character)parameters.Figure;
										AbilityCard abilityCard = await AbilityCmd.SelectAbilityCard(character, CardState.Lost,
											hintText: "Select a lost card to recover");
										if(abilityCard != null)
										{
											await AbilityCmd.ReturnToHand(abilityCard);
										}
									},
									effectType: EffectType.SelectableMandatory,
									effectButtonParameters: new IconEffectButton.Parameters(Icons.RecoverCard),
									effectInfoViewParameters: new AbilityCardEffectInfoView.Parameters(this)
								);

							ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription healSubscription =
								ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
									subscriptionParameters => true,
									async subscriptionParameters =>
									{
										ActionState actionState = new ActionState(parameters.Figure,
											[HealAbility.Builder().WithHealValue(5).WithTarget(Target.Self).Build()]);
										await actionState.Perform();
									},
									effectType: EffectType.SelectableMandatory,
									effectButtonParameters: new IconEffectButton.Parameters(Icons.Heal),
									effectInfoViewParameters: new AbilityCardEffectInfoView.Parameters(this)
								);

							await AbilityCmd.GenericChoice(state.Performer,
								[recoverCardSubscription, healSubscription], hintText: "Recover a card or Heal 5?");

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.JustBeforeSufferDamageEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.857998f)))
				.Build())
		];

		protected override int XP => 1;
		protected override bool Persistent => true;
		protected override bool Loss => true;
		protected override bool Unrecoverable => true;
	}
}