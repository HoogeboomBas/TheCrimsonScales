using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class StormyRetribution : TempestCardModel<StormyRetribution.CardTop, StormyRetribution.CardBottom>
{
	public override string Name => "Stormy Retribution";
	public override int Level => 5;
	public override int Initiative => 77;
	protected override int AtlasIndex => 19;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red)
						]
					), new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), this, new Vector2(0.7568428f, 0.12742384f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.East).Add(Direction.East), this, new Vector2(0.85677963f, 0.25096956f)))
				.WithDuringAttackSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(2, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")),

					LoseSparksConsumeElementSubscription<ScenarioEvents.DuringAttack.Parameters>(1, Element.Light, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}"))
				])
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Dodge)
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(state =>
					LoseSparksConsumeElementConditionalAbilityCheck(state.Performer, 1, Element.Air,
						$"{Icons.Inline(Icons.GetCondition(Conditions.Dodge))}, self"))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse([Element.Air, Element.Light])];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Dodge)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DodgeUsedEvent.Subscribe(state, this,
						parameters => parameters.DodgeUser == state.Performer,
						async parameters =>
						{
							await AbilityCmd.SufferDamage(state, parameters.Attacker, 3);
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DodgeUsedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}