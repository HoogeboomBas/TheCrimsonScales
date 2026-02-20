using System.Collections.Generic;
using Godot;

public class RapidFire : BombardCardModel<RapidFire.CardTop, RapidFire.CardBottom>
{
	public override string Name => "Rapid Fire";
	public override int Level => 2;
	public override int Initiative => 41;
	protected override int AtlasIndex => 13;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
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

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Air)];
		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithTargets(3, new TargetsSquare(this, new Vector2(0.49925107f, 0.77089477f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.7098483f, 0.77089477f)))
				.Build())
		];
	}
}