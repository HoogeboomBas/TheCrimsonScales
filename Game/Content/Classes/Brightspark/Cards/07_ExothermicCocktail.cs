using System.Collections.Generic;
using Godot;

public class ExothermicCocktail : BrightsparkCardModel<ExothermicCocktail.CardTop, ExothermicCocktail.CardBottom>
{
	public override string Name => "Exothermic Cocktail";
	public override int Level => 1;
	public override int Initiative => 45;
	protected override int AtlasIndex => 7;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.6191593f, 0.14467353f)))
				.WithAOEPattern(new AOEPattern(
						[
							new AOEHex(Vector2I.Zero, AOEHexType.Gray),
							new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Empty),
							new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.NorthEast), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
							new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.SouthEast), AOEHexType.Red),
						]
					),
					new AOEHexMark(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.NorthEast), this,
						new Vector2(0.6511111f, 0.2245968f)),
					new AOEHexMark(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.SouthEast), this,
						new Vector2(0.6505704f, 0.34698993f)))
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
		public override int XP => 1;
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.61787784f, 0.7682539f)))
				.Build())
		];
	}
}