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
			new AbilityCardAbility(new AttackAbility(3, range: 2, push: 2))
		];
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder().WithRange(1).Build()),
			new AbilityCardAbility(new MoveAbility(1))
		];
	}
}