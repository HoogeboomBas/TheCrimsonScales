using System.Collections.Generic;

public class UnbreakablePosition : BombardCardModel<UnbreakablePosition.CardTop, UnbreakablePosition.CardBottom>
{
	public override string Name => "Unbreakable Position";
	public override int Level => 5;
	public override int Initiative => 15;
	protected override int AtlasIndex => 18;

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