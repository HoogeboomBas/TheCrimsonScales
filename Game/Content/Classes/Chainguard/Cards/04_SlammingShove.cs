using System.Collections.Generic;
using Godot;

public class SlammingShove : ChainguardCardModel<SlammingShove.CardTop, SlammingShove.CardBottom>
{
	public override string Name => "Slamming Shove";
	public override int Level => 1;
	public override int Initiative => 25;
	protected override int AtlasIndex => 12 - 4;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.4923463f, 0.18387413f)))
				.WithPush(2)
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.6214893f, 0.4041298f)))
				.Build()),
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.619809f, 0.72228116f)))
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.Build()),
		];
	}
}