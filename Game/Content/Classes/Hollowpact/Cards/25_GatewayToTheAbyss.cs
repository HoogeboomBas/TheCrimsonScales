using System.Collections.Generic;

public class GatewayToTheAbyss : HollowpactCardModel<GatewayToTheAbyss.CardTop, GatewayToTheAbyss.CardBottom>
{
	public override string Name => "Gateway to the Abyss";
	public override int Level => 7;
	public override int Initiative => 66;
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