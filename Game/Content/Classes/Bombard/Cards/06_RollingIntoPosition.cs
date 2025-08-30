using System.Collections.Generic;

public class RollingIntoPosition : BombardCardModel<RollingIntoPosition.CardTop, RollingIntoPosition.CardBottom>
{
	public override string Name => "Rolling Into Position";
	public override int Level => 1;
	public override int Initiative => 14;
	protected override int AtlasIndex => 6;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(1).Build()),
			new AbilityCardAbility(ShieldAbility.Builder().WithShieldValue(1).Build())
		];

		protected override int XP => 1;
		protected override bool Round => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(4).Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Immobilize)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];
	}
}