using System.Collections.Generic;
using Godot;

public class ForcefulBolt : BombardCardModel<ForcefulBolt.CardTop, ForcefulBolt.CardBottom>
{
	public override string Name => "Forceful Bolt";
	public override int Level => 1;
	public override int Initiative => 63;
	protected override int AtlasIndex => 3;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.39117384f, 0.29026878f)))
				.WithRange(2, new RangeSquare(this, new Vector2(0.5999205f, 0.29026878f)))
				.WithPush(2)
				.Build())
		];
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(1, new MoveSquare(this, new Vector2(0.6240878f, 0.82054216f)))
				.Build())
		];
	}
}