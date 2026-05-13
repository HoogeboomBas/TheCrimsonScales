using System.Collections.Generic;

public class StalkingQuarry : HollowpactCardModel<StalkingQuarry.CardTop, StalkingQuarry.CardBottom>
{
	public override string Name => "Stalking Quarry";
	public override int Level => 5;
	public override int Initiative => 14;
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