using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class EntropyUnleashed : HollowpactLevelUpCardModel<EntropyUnleashed.CardTop, EntropyUnleashed.CardBottom>
{
	public override string Name => "Entropy Unleashed";
	public override int Level => 8;
	public override int Initiative => 28;
	protected override int AtlasIndex => 12;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(VoidsightAbilityBuilder().Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.43555552f, 0.24999997f)))
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red),
				]))
				.WithDuringAttackSubscriptions([
					LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(2,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAddCondition(Conditions.Poison1);

							await GDTask.CompletedTask;
						},
						new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Damage)}, {Icons.Inline(Icons.GetCondition(Conditions.Poison1))}")),

					ScenarioEvents.DuringAttack.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Damage)}"))
				])
				.Build()),
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(5)
				.WithTargets(3, new TargetsSquare(this, new Vector2(0.52105516f, 0.6083333f)))
				.WithTarget(Target.Allies | Target.Enemies)
				.WithRange(3, new RangeSquare(this, new Vector2(0.6999999f, 0.6397003f)))
				.WithConditions([Conditions.Regenerate, Conditions.Curse])
				.WithCustomGetTargets((state, figures) =>
				{
					// Always add all the enemies in range
					figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer, state.AbilityRange)
						.Where(figure => figure.EnemiesWith(state.Performer)));

					if(state.UniqueTargetedFigures.Any(figure => figure.EnemiesWith(state.Performer)))
					{
						// Add allies in range if an enemy was already targeted
						figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer, state.AbilityRange)
							.Where(figure => figure.AlliedWith(state.Performer)));
					}
				})
				.Build()),

			new AbilityCardAbility(GainVoidEnergyAbilityBuilder()
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.InfuseWild()];
	}
}