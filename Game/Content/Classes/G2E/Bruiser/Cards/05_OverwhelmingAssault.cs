using System.Collections.Generic;
using Godot;

public class OverwhelmingAssault : BruiserCardModel<OverwhelmingAssault.CardTop, OverwhelmingAssault.CardBottom>
{
	public override string Name => "Overwhelming Assault";
	public override int Level => 1;
	public override int Initiative => 61;
	protected override int AtlasIndex => 5;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(7, new AttackDiamond(this, new Vector2(0.50983125f, 0.2910521f)))
				.WithRange(1)
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;

	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.61780804f, 0.77249503f)))
				.WithMoveType(MoveType.Jump)
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [ CardElementInfusion.Infuse(Element.Air) ];
	}
}