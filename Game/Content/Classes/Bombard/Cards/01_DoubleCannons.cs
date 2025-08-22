using System.Collections.Generic;
using Godot;

public class DoubleCannons : BombardCardModel<DoubleCannons.CardTop, DoubleCannons.CardBottom>
{
	public override string Name => "Double Cannons";
	public override int Level => 1;
	public override int Initiative => 60;
	protected override int AtlasIndex => 1;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(3, range: 2, rangeType: RangeType.Range, aoePattern: new AOEPattern(
			[
				new AOEHex(Vector2I.Zero, AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
			])))
		];
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(2)
				.WithRequiredRangeType(RangeType.Range)
				.Build())
		];

		protected override bool Round => true;
	}
}