using System.Collections.Generic;

public class PipeTomahawk : ChieftainCardModel<PipeTomahawk.CardTop, PipeTomahawk.CardBottom>
{
	public override string Name => "Pipe Tomahawk";
	public override int Level => 1;
	public override int Initiative => 26;
	protected override int AtlasIndex => 3;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithRange(2)
				.WithPierce(1)
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Fire];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(4).Build())
		];
	}
}