using System.Collections.Generic;
using Godot;

public class RollingIntoPosition : BombardCardModel<RollingIntoPosition.CardTop, RollingIntoPosition.CardBottom>
{
	public override string Name => "Rolling Into Position";
	public override int Level => 1;
	public override int Initiative => 14;
	protected override int AtlasIndex => 6;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(1, new MoveCircle(this, new Vector2(0.6239878f, 0.2347049f)))
				.Build()),

			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.Build())
		];

		public override int XP => 1;
		public override bool Round => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.6179879f, 0.7131996f)))
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Immobilize)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];
	}
}