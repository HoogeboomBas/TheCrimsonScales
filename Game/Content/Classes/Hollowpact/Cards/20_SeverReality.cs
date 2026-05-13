using System.Collections.Generic;

public class SeverReality : HollowpactCardModel<SeverReality.CardTop, SeverReality.CardBottom>
{
	public override string Name => "Sever Reality";
	public override int Level => 5;
	public override int Initiative => 78;
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