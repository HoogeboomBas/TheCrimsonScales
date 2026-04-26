using System.Collections.Generic;

public class TestCard : BombardCardModel<TestCard.CardTop, TestCard.CardBottom>
{
	public override string Name => "Test Card";
	public override int Level => 1;
	public override int Initiative => 01;
	protected override int AtlasIndex => 0;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder().WithDamage(100).WithTargets(100).WithInfiniteRange().Build())
		];
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(100).WithMoveType(MoveType.Jump).Build())
		];
	}
}