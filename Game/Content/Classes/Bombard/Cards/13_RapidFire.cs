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
					AttackAbility.Builder()
						.WithDamage(4)
						.WithRangeType(RangeType.Range)
						.WithTargetHex(hex)
						.Build()
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
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithTargets(3)
				.WithRange(3)
				.Build())
		];
	}
}