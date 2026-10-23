using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SparkConduit : TempestCardModel<SparkConduit.CardTop, SparkConduit.CardBottom>
{
	public override string Name => "Spark Conduit";
	public override int Level => 5;
	public override int Initiative => 73;
	protected override int AtlasIndex => 20;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.44879138f, 0.12742384f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.6596582f, 0.12676981f)))
				.WithDuringAttackSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")),

					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustRange(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Range)}")),

					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Wound1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Wound1)))),


					LoseSparksConsumeElementSubscription<ScenarioEvents.DuringAttack.Parameters>(1, Element.Light, async parameters =>
					{
						parameters.AbilityState.AdjustTargets(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Targets)}"))
				])
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse([Element.Air, Element.Light])];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GainSparkAbilityBuilder(2).Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async _ =>
						{
							await GainSpark(state);
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}