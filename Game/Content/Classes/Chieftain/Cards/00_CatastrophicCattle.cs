using System.Collections.Generic;
using Godot;

public class CatastrophicCattle : ChieftainCardModel<CatastrophicCattle.CardTop, CatastrophicCattle.CardBottom>
{
	public override string Name => "Catastrophic Cattle";
	public override int Level => 1;
	public override int Initiative => 88;
	protected override int AtlasIndex => 0;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Fighting Bull")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/fighting_bull_AI.png")
				.WithHealth(4, new SummonHealthSquare(this, new Vector2(0.44718847f, 0.23893806f)))
				.WithMove(2, new SummonMoveSquare(this, new Vector2(0.67835045f, 0.23893806f)))
				.WithAttack(2, new SummonAttackSquare(this, new Vector2(0.44718847f, 0.31493726f)))
				.WithTraits(new MountTrait(), new PushTrait(1))
				.Build()
			),
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.6224222f, 0.723211f)))
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];
	}
}