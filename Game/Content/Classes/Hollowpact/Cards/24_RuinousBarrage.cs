using System.Collections.Generic;

public class RuinousBarrage : HollowpactCardModel<RuinousBarrage.CardTop, RuinousBarrage.CardBottom>
{
	public override string Name => "Ruinous Barrage";
	public override int Level => 7;
	public override int Initiative => 38;
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