using System.Collections.Generic;
using Godot;

public class ChainGrapnel : BombardCardModel<ChainGrapnel.CardTop, ChainGrapnel.CardBottom>
{
	public override string Name => "Chain Grapnel";
	public override int Level => 1;
	public override int Initiative => 46;
	protected override int AtlasIndex => 9;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			//new AbilityCardAbility(new AttackAbility(2, range: 3, pull: 2, conditions: [Conditions.Immobilize]))
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRange(3)
				.WithPush(2)
				.WithConditions(Conditions.Immobilize)
				.Build()) //TODO: Card art is wrong, it's supposed to be a pull. Leaving it like this for now or it will cause confusion.
		];

		protected override int XP => 1;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ProjectileAbility.Builder()
				.WithGetAbilities(hex =>
				[
					ConditionAbility.Builder()
						.WithConditions(Conditions.Immobilize)
						.WithAOEPattern(new AOEPattern(
							[
								new AOEHex(Vector2I.Zero, AOEHexType.Red),
								new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
								new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red)
							]
						))
						.WithTargetHex(hex)
						.Build()
				])
				.WithAbilityCardSide(this)
				.WithRange(4)
				.Build())
		];

		protected override int XP => 1;
		protected override bool Persistent => true;
	}
}