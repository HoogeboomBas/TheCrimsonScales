using System.Collections.Generic;

public class ShroudedGrasp : HollowpactCardModel<ShroudedGrasp.CardTop, ShroudedGrasp.CardBottom>
{
	public override string Name => "Shrouded Grasp";
	public override int Level => 2;
	public override int Initiative => 23;
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