using System.Collections.Generic;

public class EntropyUnleashed : HollowpactCardModel<EntropyUnleashed.CardTop, EntropyUnleashed.CardBottom>
{
	public override string Name => "Entropy Unleashed";
	public override int Level => 8;
	public override int Initiative => 28;
	protected override int AtlasIndex => 0;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[

		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[

		];
	}
}