using System.Collections.Generic;
using Godot;

public class VenomousMayhem : ChieftainCardModel<VenomousMayhem.CardTop, VenomousMayhem.CardBottom>
{
	public override string Name => "Venomous Mayhem";
	public override int Level => 6;
	public override int Initiative => 92;
	protected override int AtlasIndex => 22;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Cottonmouth Snake")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/cottonmouth_snake_AI.png")
				.WithHealth(5, new SummonHealthSquare(this, new Vector2(0.4466124f, 0.24022135f)))
				.WithMove(4, new SummonMoveSquare(this, new Vector2(0.6772746f, 0.24022135f)))
				.WithAttack(1, new SummonAttackSquare(this, new Vector2(0.4466124f, 0.31622052f)))
				.WithTraits(
					new ApplyConditionTrait(Conditions.Poison1),
					new ApplyConditionTrait(Conditions.Immobilize)
				)
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
			new AbilityCardAbility(AttackAbility.Builder().WithDamage(3).WithConditions(Conditions.Poison1).Build()),
		];
	}
}