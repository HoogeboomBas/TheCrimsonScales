using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ThermalWeaving : RimehearthCardModel<ThermalWeaving.CardTop, ThermalWeaving.CardBottom>
{
	public override string Name => "Thermal Weaving";
	public override int Level => 9;
	public override int Initiative => 32;
	protected override int AtlasIndex => 27;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest).Add(Direction.NorthWest), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest).Add(Direction.NorthEast), AOEHexType.Red),
						]
					), new AOEHexMark(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.NorthWest), this, new Vector2(0.6551259f, 0.2545018f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), this, new Vector2(0.85560346f, 0.25429362f)))
				.WithDuringAttackSubscriptions(
				[
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAddCondition(Conditions.Wound1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.GetCondition(Conditions.Wound1))}")),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Ice,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAddCondition(Conditions.Chill);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.GetCondition(Conditions.Chill))}")),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(
						[CardElementConsumption.Consume(Element.Fire), CardElementConsumption.Consume(Element.Ice)],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Brittle);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Brittle))}"))
				])
				.Build()),
		];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Subscribe(state, this,
						parameters => parameters.RetaliatingFigure == state.Performer &&
						              RangeHelper.Distance(parameters.AbilityState.Performer.Hex, state.Performer.Hex) <= 1 &&
						              GameController.Instance.ElementManager.GetState(Element.Fire) is ElementState.Strong or ElementState.Waning,
						async parameters =>
						{
							parameters.AdjustRetaliate(2);

							await GDTask.CompletedTask;
						});

					ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer &&
						              GameController.Instance.ElementManager.GetState(Element.Fire) is ElementState.Strong or ElementState.Waning,
						parameters =>
						{
							parameters.AddRetaliate(2, 1);
						}
					);

					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && parameters.FromAttack &&
						              GameController.Instance.ElementManager.GetState(Element.Ice) is ElementState.Strong or ElementState.Waning,
						async parameters =>
						{
							parameters.AdjustShield(2);

							await GDTask.CompletedTask;
						});

					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer &&
						              GameController.Instance.ElementManager.GetState(Element.Ice) is ElementState.Strong or ElementState.Waning,
						parameters =>
						{
							parameters.AdjustShield(2);
						});

					ScenarioEvents.FinishElementConsumedEvent.Subscribe(state, this,
						parameters => parameters.ConsumedElement is Element.Fire or Element.Ice,
						async _ =>
						{
							ScenarioCheckEvents.ShieldCheckEvent.FireChangedEvent();
							ScenarioCheckEvents.RetaliateCheckEvent.FireChangedEvent();
							await GDTask.CompletedTask;
						}, EffectType.Visuals);

					ScenarioEvents.FinishElementInfusedEvent.Subscribe(state, this,
						parameters => parameters.InfusedElement is Element.Fire or Element.Ice,
						async _ =>
						{
							ScenarioCheckEvents.ShieldCheckEvent.FireChangedEvent();
							ScenarioCheckEvents.RetaliateCheckEvent.FireChangedEvent();

							await GDTask.CompletedTask;
						}, EffectType.Visuals);

					ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
						_ => GameController.Instance.ElementManager.GetState(Element.Fire) is ElementState.Inert &&
						     GameController.Instance.ElementManager.GetState(Element.Ice) is ElementState.Inert,
						async _ =>
						{
							await state.ActionState.RequestDiscardOrLose();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioEvents.FinishElementConsumedEvent.Unsubscribe(state, this);
					ScenarioEvents.FinishElementInfusedEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse([Element.Fire, Element.Ice])];
		public override int XP => 1;
		public override bool Persistent => true;
	}
}