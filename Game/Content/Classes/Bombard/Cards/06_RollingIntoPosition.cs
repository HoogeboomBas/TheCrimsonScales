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
			new AbilityCardAbility(new MoveAbility(1)),
			new AbilityCardAbility(new ShieldAbility(1))
		];

		protected override int XP => 1;
		protected override bool Round => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(4)),
			new AbilityCardAbility(new ConditionAbility([Conditions.Immobilize], target: Target.Self, mandatory: true))
		];
	}
}