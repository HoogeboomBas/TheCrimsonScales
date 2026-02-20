using System.Collections.Generic;
using Godot;

public class CeremonialDance : ChieftainCardModel<CeremonialDance.CardTop, CeremonialDance.CardBottom>
{
	public override string Name => "Ceremonial Dance";
	public override int Level => 2;
	public override int Initiative => 23;
	protected override int AtlasIndex => 14;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithTargets(3, new TargetsSquare(this, new Vector2(0.55794144f, 0.27738613f)))
				.WithConditions(Conditions.Muddle)
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder().WithDistance(2).Build()
				])
				.WithTarget(Target.MustTargetCharacters | Target.SelfOrAllies | Target.TargetAll)
				.WithRange(3)
				.Build()
			)
		];
	}
}