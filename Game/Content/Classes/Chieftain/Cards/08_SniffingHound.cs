using System.Collections.Generic;
using Godot;

public class SniffingHound : ChieftainCardModel<SniffingHound.CardTop, SniffingHound.CardBottom>
{
	public override string Name => "Sniffing Hound";
	public override int Level => 1;
	public override int Initiative => 80;
	protected override int AtlasIndex => 8;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Scout Dog")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/scout_dog_AI.png")
				.WithHealth(4, new SummonHealthSquare(this, new Vector2(0.5267429f, 0.19222368f)))
				.WithMove(3, new SummonMoveSquare(this, new Vector2(0.81171906f, 0.19222368f)))
				.WithAttack(1, new SummonAttackSquare(this, new Vector2(0.5267429f, 0.26822355f)))
				.WithTraits(new TrapDisarmTrait(1))
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
				.WithDistance(5, new MoveCircle(this, new Vector2(0.62056f, 0.7620452f)))
				.Build()),
		];
	}
}