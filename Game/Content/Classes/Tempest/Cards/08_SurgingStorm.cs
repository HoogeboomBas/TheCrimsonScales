using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SurgingStorm : TempestCardModel<SurgingStorm.CardTop, SurgingStorm.CardBottom>
{
	public override string Name => "Surging Storm";
	public override int Level => 1;
	public override int Initiative => 35;
	protected override int AtlasIndex => 8;

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
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red)
					]
				))
				.WithDuringAttackSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AbilitySetHasAdvantage();

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters("advantage")),

					LoseSparksConsumeElementSubscription<ScenarioEvents.DuringAttack.Parameters>(1, Element.Light, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")),

					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Air, applyFunction: async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Muddle);

						await GDTask.CompletedTask;
					}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Muddle))}"))
				])
				.Build()),
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6210548f, 0.62382275f)))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Immobilize)
				.WithConditionalAbilityCheck(async state =>
				{
					bool useSpark = await LoseSparksConditionalAbilityCheck(state.Performer, 1,
						new TextEffectInfoView.Parameters(
							$"{Icons.Inline(Icons.Attack)}3, {Icons.Inline(Icons.GetCondition(Conditions.Immobilize))}, {Icons.Inline(Icons.LoseCard)}"));
					if(!useSpark)
					{
						return false;
					}

					await AbilityCmd.GainXP(state.Performer, 1);
					state.ActionState.SetOverrideLoss();
					return true;
				})
				.Build()),
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2)
				.WithRange(1)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse([Element.Air, Element.Light])];
	}
}