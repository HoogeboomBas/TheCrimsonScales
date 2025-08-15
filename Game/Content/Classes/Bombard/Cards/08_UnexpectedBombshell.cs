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
			new AbilityCardAbility(new ProjectileAbility(4,
				hex =>
				[
					new AttackAbility(2, conditions: [Conditions.Stun], rangeType: RangeType.Range, targetHex: hex,
						afterAttackPerformedSubscriptions:
						[
							ScenarioEvent<ScenarioEvents.AfterAttackPerformed.Parameters>.Subscription.New(
								applyFunction: async applyParameters =>
								{
									List<Hex> hexes = new List<Hex>();
									RangeHelper.FindHexesInRange(applyParameters.AbilityState.Target.Hex, 1, false, hexes);
									List<Figure> enemies = new List<Figure>();
									foreach(Hex neighbourHex in hexes)
									{
										foreach(Figure figure in neighbourHex.GetHexObjectsOfType<Figure>())
										{
											if(figure != applyParameters.AbilityState.Target && applyParameters.AbilityState.Performer.EnemiesWith(figure))
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
						]),
				],
				this)
			)
		];

		protected override int XP => 1;
		protected override bool Persistent => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new PullSelfAbility(4, range: 5))
		];
	}
}