using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Hurricane : TempestCardModel<Hurricane.CardTop, Hurricane.CardBottom>
{
	public override string Name => "Hurricane";
	public override int Level => 8;
	public override int Initiative => 82;
	protected override int AtlasIndex => 25;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithPush(2)
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red)
						]
					), new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), this, new Vector2(0.7752924f, 0.20831026f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.East).Add(Direction.NorthEast), this, new Vector2(0.8257369f, 0.2692521f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.East).Add(Direction.East), this, new Vector2(0.87548125f, 0.33019394f)))
				.WithDuringAttackSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(2, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);
						parameters.AbilityState.AbilitySetHasAdvantage();

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}, advantage")),

					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Air, applyFunction: async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Muddle);

						await GDTask.CompletedTask;
					}, effectInfoViewParameters: new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Muddle)))),

					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light, applyFunction: async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);

						await GDTask.CompletedTask;
					}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}"))
				])
				.Build()),
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6210789f, 0.6170284f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Dodge)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DodgeUsedEvent.Subscribe(state, this,
						parameters => parameters.DodgeUser == state.Performer,
						async parameters =>
						{
							await AbilityCmd.InfuseElement(state, Element.Air);

							await AbilityCmd.AddConditions(state, parameters.DodgeUser, [Conditions.Dodge, Conditions.Strengthen]);

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DodgeUsedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.50056416f, 0.897507f), GainXP))
				.Build())
		];

		public override bool Persistent => true;
	}
}