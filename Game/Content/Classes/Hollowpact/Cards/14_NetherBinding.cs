using System.Collections.Generic;

public class NetherBinding : HollowpactCardModel<NetherBinding.CardTop, NetherBinding.CardBottom>
{
	public override string Name => "Nether Binding";
	public override int Level => 2;
	public override int Initiative => 64;
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