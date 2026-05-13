using System.Collections.Generic;

public class VoidEnhancedArmory : HollowpactCardModel<VoidEnhancedArmory.CardTop, VoidEnhancedArmory.CardBottom>
{
	public override string Name => "Void-Enhanced Armory";
	public override int Level => 4;
	public override int Initiative => 17;
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