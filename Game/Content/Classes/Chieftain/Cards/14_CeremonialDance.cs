using System.Collections.Generic;

public class CeremonialDance : ChieftainCardModel<CeremonialDance.CardTop, CeremonialDance.CardBottom>
{
	public override string Name => "Ceremonial Dance";
	public override int Level => 2;
	public override int Initiative => 23;
	protected override int AtlasIndex => 14;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithTargets(3)
				.WithConditions(Conditions.Muddle)
				.Build())
		];

		protected override int XP => 1;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state => 
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