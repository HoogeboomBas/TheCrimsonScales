using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class RadiantExplosion : TempestCardModel<RadiantExplosion.CardTop, RadiantExplosion.CardBottom>
{
	public override string Name => "Radiant Explosion";
	public override int Level => 7;
	public override int Initiative => 18;
	protected override int AtlasIndex => 23;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.4447872f, 0.12787785f)))
				.WithPierce(3, new PierceSquare(this, new Vector2(0.6659668f, 0.12642382f)))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(applyFunction: async parameters =>
						{
							await ((AttackAbility.State)parameters.AbilityState).SetPerformHex(hexes =>
							{
								hexes.AddRange(GameController.Instance.Map.Hexes.Select(hex => hex.Value)
									.Where(hex => hex.HasHexObjectOfType<TempestSpark>()));
							});
						}, effectType: EffectType.Selectable,
						effectButtonParameters: new TextEffectButton.Parameters(
							$"Perform this ability as if you were occupying a hex with {Icons.Inline(Tempest.SparkTokenIconPath)}")))
				.WithDuringAttackSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(2, async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Wound1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Wound1)))),

					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);

							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")),

					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(2, async parameters =>
						{
							parameters.AbilityState.SetCustomValue(this, "SufferDamage", true);

							await AbilityCmd.GainXP(parameters.Performer, 1);

							parameters.AbilityState.ActionState.SetOverrideLoss();
						},
						new TextEffectInfoView.Parameters(
							$"All enemies adjacent to the target suffer {Icons.Inline(Icons.Damage)}3, {Icons.Inline(Icons.LoseCard)}"))
				])
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(
						parameters => parameters.AbilityState.GetCustomValue<bool>(this, "SufferDamage"),
						async parameters =>
						{
							foreach(Figure figure in RangeHelper.GetFiguresInRange(parameters.AbilityState.Target, 1)
								        .Where(figure => parameters.Performer.EnemiesWith(figure)))
							{
								await AbilityCmd.SufferDamage(parameters.AbilityState, figure, 3);
							}
						}))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer, 1).Where(figure => state.Performer.AlliedWith(figure)));
					figures.AddRange(GameController.Instance.Map.Hexes.Select(hex => hex.Value)
						.Where(hex => hex.HasHexObjectOfType<TempestSpark>()).SelectMany(hex => hex.GetFigures()));
					figures.Add(state.Performer);
				})
				.WithConditions(Conditions.Regenerate)
				.WithDuringHealSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringHeal.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustHealValue(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Heal)}")),

					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Air, applyFunction: async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Dodge);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"Add {Icons.Inline(Icons.GetCondition(Conditions.Dodge))} to the next heal")),
				])
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Hex hex in state.TargetedHexes)
					{
						await AbilityCmd.LootHex(state.Performer, hex);
					}
				})
				.Build())
		];

		public override int XP => 1;
	}
}