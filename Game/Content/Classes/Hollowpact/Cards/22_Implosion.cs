using System.Collections.Generic;

public class Implosion : HollowpactCardModel<Implosion.CardTop, Implosion.CardBottom>
{
	public override string Name => "Implosion";
	public override int Level => 6;
	public override int Initiative => 49;
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