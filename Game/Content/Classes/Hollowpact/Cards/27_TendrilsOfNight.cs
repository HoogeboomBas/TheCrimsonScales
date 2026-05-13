using System.Collections.Generic;

public class TendrilsOfNight : HollowpactCardModel<TendrilsOfNight.CardTop, TendrilsOfNight.CardBottom>
{
	public override string Name => "Tendrils of Night";
	public override int Level => 8;
	public override int Initiative => 44;
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