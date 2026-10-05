using System.Collections.Generic;
using Godot;

public class SpareDagger : BruiserCardModel<SpareDagger.CardTop, SpareDagger.CardBottom>
{
	public override string Name => "Spare Dagger";
	public override int Level => 1;
	public override int Initiative => 27;
	protected override int AtlasIndex => 8;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithRange(3)
				.Build())
		];
	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.50983125f, 0.2910521f)))
				.WithRange(1)
				.WithPierce(1, new PierceSquare(this, new Vector2(0.50983125f, 0.2910521f)))
				.Build())
		];
	}
}