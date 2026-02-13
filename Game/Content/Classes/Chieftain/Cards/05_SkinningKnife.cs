using System.Collections.Generic;
using Godot;

public class SkinningKnife : ChieftainCardModel<SkinningKnife.CardTop, SkinningKnife.CardBottom>
{
	public override string Name => "Skinning Knife";
	public override int Level => 1;
	public override int Initiative => 54;
	protected override int AtlasIndex => 5;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.619084f, 0.24810435f)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];

		public override int XP => 1;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder().WithRange(1).Build())
		];
	}
}