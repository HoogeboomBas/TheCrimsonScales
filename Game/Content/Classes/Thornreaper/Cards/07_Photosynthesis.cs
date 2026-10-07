using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Photosynthesis : ThornreaperCardModel<Photosynthesis.CardTop, Photosynthesis.CardBottom>
{
	public override string Name => "Restoring Faith";
	public override int Level => 1;
	public override int Initiative => 64;
	protected override int AtlasIndex => 13 - 6;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.Build())
		];
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