using System.Collections.Generic;

public class NoEscape : HollowpactCardModel<NoEscape.CardTop, NoEscape.CardBottom>
{
	public override string Name => "No Escape";
	public override int Level => 9;
	public override int Initiative => 57;
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