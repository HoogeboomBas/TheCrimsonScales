using System.Collections.Generic;
using System.Linq;
using Godot;

public class EntropyUnleashed : HollowpactCardModel<EntropyUnleashed.CardTop, EntropyUnleashed.CardBottom>
{
	public override string Name => "Entropy Unleashed";
	public override int Level => 8;
	public override int Initiative => 28;
	protected override int AtlasIndex => 0;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(VoidsightAbilityBuilder().Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.41209206f, 0.1612586f)))
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
				.WithTargets(3, new TargetsSquare(this, new Vector2(0.41209206f, 0.1612586f)))
				.WithTarget(Target.Allies | Target.Enemies)
				.WithRange(3, new RangeSquare(this, new Vector2(0.41209206f, 0.1612586f)))
				.WithConditions([Conditions.Regenerate, Conditions.Curse])
				.WithCustomGetTargets((state, figures) =>
				{
					// Always add all the enemies in range
					figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer, state.AbilityRange).Where(figure => figure.EnemiesWith(state.Performer)));

					if(state.UniqueTargetedFigures.Count(figure => figure.AlliedWith(state.Performer)) < 2)
					{
						// Add allies in range if less than 2 targeted
						figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer, state.AbilityRange).Where(figure => figure.AlliedWith(state.Performer)));
					}
				})
				.Build()),

			new AbilityCardAbility(GainVoidEnergyAbilityBuilder()
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.InfuseWild()];
	}
}