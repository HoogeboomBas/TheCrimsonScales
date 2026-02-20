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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackCircle(this, new Vector2(0.34399113f, 0.28388575f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.55398846f, 0.28388575f)))
				.WithPull(2)
				.WithConditions(Conditions.Immobilize)
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
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
				.WithRange(4, new ProjectileRangeSquare(this, new Vector2(0.3408154f, 0.6195522f), EnhancementCostType.MultiTarget))
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}
}