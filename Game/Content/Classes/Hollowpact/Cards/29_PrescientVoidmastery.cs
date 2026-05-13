using System.Collections.Generic;

public class PrescientVoidmastery : HollowpactCardModel<PrescientVoidmastery.CardTop, PrescientVoidmastery.CardBottom>
{
	public override string Name => "Prescient Voidmastery";
	public override int Level => 9;
	public override int Initiative => 11;
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