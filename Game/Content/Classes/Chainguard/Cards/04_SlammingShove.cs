using System.Collections.Generic;

public class SlammingShove : ChainguardCardModel<SlammingShove.CardTop, SlammingShove.CardBottom>
{
	public override string Name => "Slamming Shove";
	public override int Level => 1;
	public override int Initiative => 25;
	protected override int AtlasIndex => 12 - 4;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithPush(2)
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.Build()),
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.Build()),
		];
	}
}