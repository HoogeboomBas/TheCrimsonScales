using System.Collections.Generic;

public class ForcefulBolt : BombardCardModel<ForcefulBolt.CardTop, ForcefulBolt.CardBottom>
{
	public override string Name => "Forceful Bolt";
	public override int Level => 1;
	public override int Initiative => 63;
	protected override int AtlasIndex => 3;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithRange(2)
				.WithPush(2)
				.Build())
		];
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder().WithRange(1).Build()),
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(1).Build())
		];
	}
}