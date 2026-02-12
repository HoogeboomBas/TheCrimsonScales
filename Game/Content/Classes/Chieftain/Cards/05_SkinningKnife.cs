using System.Collections.Generic;

public class SkinningKnife : ChieftainCardModel<SkinningKnife.CardTop, SkinningKnife.CardBottom>
{
	public override string Name => "Skinning Knife";
	public override int Level => 1;
	public override int Initiative => 54;
	protected override int AtlasIndex => 5;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Earth];

		protected override int XP => 1;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder().WithRange(1).Build())
		];
	}
}