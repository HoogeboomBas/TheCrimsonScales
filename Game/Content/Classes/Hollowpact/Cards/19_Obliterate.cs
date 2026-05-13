using System.Collections.Generic;

public class Obliterate : HollowpactCardModel<Obliterate.CardTop, Obliterate.CardBottom>
{
	public override string Name => "Obliterate";
	public override int Level => 4;
	public override int Initiative => 13;
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