using System.Collections.Generic;

public class RapidFire : BombardCardModel<RapidFire.CardTop, RapidFire.CardBottom>
{
	public override string Name => "Rapid Fire";
	public override int Level => 2;
	public override int Initiative => 41;
	protected override int AtlasIndex => 13;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ProjectileAbility.Builder().WithGetAbilities(hex =>
				[
					new AttackAbility(4, rangeType: RangeType.Range, targetHex: hex)
				])
				.WithAbilityCardSide(this)
				.WithRange(3)
				.WithTargets(3)
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Air];
		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(1, targets: 3, range: 3))
		];
	}
}