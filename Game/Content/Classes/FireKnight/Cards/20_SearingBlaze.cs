using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SearingBlaze : FireKnightLevelUpCardModel<SearingBlaze.CardTop, SearingBlaze.CardBottom>
{
	public override string Name => "Searing Blaze";
	public override int Level => 5;
	public override int Initiative => 68;
	protected override int AtlasIndex => 8;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRange(2)
				.WithConditions(Conditions.Wound1)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red),
					]
				))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							await AbilityCmd.GenericChoice(parameters.Performer,
							[
								ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
									applyFunction: async applyParameters =>
									{
										((AttackAbility.State)parameters.AbilityState).AbilityAdjustRange(1);

										await GDTask.CompletedTask;
									},
									effectButtonParameters: new IconEffectButton.Parameters(Icons.Range),
									effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Range)}"),
									effectType: EffectType.Selectable
								),
								ScenarioEvents.GenericChoice.Subscription.New(canApplyFunction: canApplyParameters => true,
									applyFunction: async applyParameters =>
									{
										((AttackAbility.State)parameters.AbilityState).AbilitySetHasAdvantage();

										await GDTask.CompletedTask;
									},
									effectButtonParameters: new TextEffectButton.Parameters("adv."),
									effectInfoViewParameters: new TextEffectInfoView.Parameters("Gain advantage"),
									effectType: EffectType.Selectable
								),
							], hintText: "Choose an effect to apply");
						}
					)
				)
				.WithAbilityEndedSubscription(
					ScenarioEvents.AbilityEnded.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>() && parameters.AbilityState.Performed,
						async parameters =>
						{
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}
					)
				)
				.Build()),
		];

		protected override IEnumerable<Element> Elements => [Element.Fire];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					bool bonusOnWoundedTarget = false;
					bool woundAdded = false;

					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Performer == state.Performer && canApplyParameters.AbilityState.Target.HasCondition(Conditions.Wound1) &&
							!bonusOnWoundedTarget,
						async parameters =>
						{
							bonusOnWoundedTarget = true;
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							parameters.AbilityState.SingleTargetSetHasAdvantage();

							await GDTask.CompletedTask;
						});

					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						canApplyParameters => true,
						async applyParameters =>
						{
							bonusOnWoundedTarget = false;

							await GDTask.CompletedTask;
						}
					);

					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						canApplyParameters => canApplyParameters.Performer == state.Performer && !woundAdded,
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Wound1))}")
					));

					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						canApplyParameters => true,
						async applyParameters =>
						{
							woundAdded = false;

							await GDTask.CompletedTask;
						}
					);
						
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}