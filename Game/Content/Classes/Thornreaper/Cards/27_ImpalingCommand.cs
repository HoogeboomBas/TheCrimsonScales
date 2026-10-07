using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class ImpalingCommand : ThornreaperLevelUpCardModel<ImpalingCommand.CardTop, ImpalingCommand.CardBottom>
{
	public override string Name => "Righteous Atonement";
	public override int Level => 8;
	public override int Initiative => 20;
	protected override int AtlasIndex => 15 - 12;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
			.WithDamage(2)
			.Build())
		];

		public override bool Round => true;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
			.WithDistance(2)
			.Build())
		];
	}
}