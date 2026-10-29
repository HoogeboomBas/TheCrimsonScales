using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class GolsTonic : IncarnateCardModel<GolsTonic.CardTop, GolsTonic.CardBottom>
{
	public override string Name => "Gol's Tonic";
	public override int Level => 1;
	public override int Initiative => 11;
	protected override int AtlasIndex => 9;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.2704956f, 0.29020473f)))
				.WithRange(2)
				.WithRangeType(RangeType.Melee)
				.WithConditions([Incarnate.Enfeeble, Incarnate.Enfeeble])
				.Build())
		];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(5, new HealDiamondPlus(this, new Vector2(0.39674714f, 0.7386042f)))
				.WithRange(2)
				.WithConditions([Incarnate.Empower, Incarnate.Empower])
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}