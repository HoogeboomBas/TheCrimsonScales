using System.Collections.Generic;

public class MajesticMalevolence : HollowpactCardModel<MajesticMalevolence.CardTop, MajesticMalevolence.CardBottom>
{
	public override string Name => "Majestic Malevolence";
	public override int Level => 3;
	public override int Initiative => 89;
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