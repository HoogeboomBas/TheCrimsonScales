using System.Collections.Generic;

public class EnduringDarkness : HollowpactCardModel<EnduringDarkness.CardTop, EnduringDarkness.CardBottom>
{
	public override string Name => "Enduring Darkness";
	public override int Level => 6;
	public override int Initiative => 26;
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