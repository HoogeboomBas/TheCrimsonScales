using System.Collections.Generic;

public class UnexpectedBombshell : BombardCardModel<UnexpectedBombshell.CardTop, UnexpectedBombshell.CardBottom>
{
	public override string Name => "Unexpected Bombshell";
	public override int Level => 1;
	public override int Initiative => 85;
	protected override int AtlasIndex => 8;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ProjectileAbility.Builder()
				.WithGetAbilities(hex =>
				[
					AttackAbility.Builder()
						.WithDamage(2)
						.WithConditions(Conditions.Stun)
						.WithRangeType(RangeType.Range)
						.WithTargetHex(hex)
						.WithAfterAttackPerformedSubscription(
							ScenarioEvents.AfterAttackPerformed.Subscription.New(
								applyFunction: async applyParameters =>
								{
									List<Hex> hexes = new List<Hex>();
									RangeHelper.FindHexesInRange(applyParameters.AbilityState.Target.Hex, 1, false, hexes);
									List<Figure> enemies = new List<Figure>();
									foreach(Hex neighbourHex in hexes)
									{
										foreach(Figure figure in neighbourHex.GetHexObjectsOfType<Figure>())
										{
											if(figure != applyParameters.AbilityState.Target &&
											   applyParameters.AbilityState.Performer.EnemiesWith(figure))
											{
												enemies.Add(figure);
											}
										}
									}

									foreach(Figure enemy in enemies)
									{
										await AbilityCmd.SufferDamage(null, enemy, 1);
									}
								})
						)
						.Build(),
				])
				.WithAbilityCardSide(this)
				.WithRange(4)
				.Build())
		];

		protected override int XP => 1;
		protected override bool Persistent => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullSelfAbility.Builder()
				.WithPullSelfValue(4)
				.WithRange(5)
				.Build())
		];
	}
}