using System.Collections.Generic;

public class SharpenedFocus : BombardCardModel<SharpenedFocus.CardTop, SharpenedFocus.CardBottom>
{
	public override string Name => "Sharpened Focus";
	public override int Level => 5;
	public override int Initiative => 32;
	protected override int AtlasIndex => 19;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			//TODO
		];
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			//TODO
		];
	}
}