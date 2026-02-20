using System.Collections.Generic;
using Godot;

public class PipeTomahawk : ChieftainCardModel<PipeTomahawk.CardTop, PipeTomahawk.CardBottom>
{
	public override string Name => "Pipe Tomahawk";
	public override int Level => 1;
	public override int Initiative => 26;
	protected override int AtlasIndex => 3;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.39152586f, 0.24792106f)))
				.WithRange(2, new RangeSquare(this, new Vector2(0.60192305f, 0.24792106f)))
				.WithPierce(1)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.618984f, 0.7672617f)))
				.Build())
		];
	}
}