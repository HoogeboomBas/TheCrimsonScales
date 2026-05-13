using System.Collections.Generic;

public class EmpoweredAssault : HollowpactCardModel<EmpoweredAssault.CardTop, EmpoweredAssault.CardBottom>
{
	public override string Name => "Empowered Assault";
	public override int Level => 3;
	public override int Initiative => 19;
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