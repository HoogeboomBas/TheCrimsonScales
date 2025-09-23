using System.Collections.Generic;
using Fractural.Tasks;

public class ProtectiveInstinct : FireKnightCardModel<ProtectiveInstinct.CardTop, ProtectiveInstinct.CardBottom>
{
	public override string Name => "Protective Instinct";
	public override int Level => 1;
	public override int Initiative => 11;
	protected override int AtlasIndex => 12 - 12;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder().WithRange(1).Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state => [ShieldAbility.Builder().WithShieldValue(1).Build()])
				.WithTarget(Target.SelfOrAllies)
				.Build())
		];

		protected override bool Round => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state => [MoveAbility.Builder().WithDistance(3).Build()])
				.WithRange(2)
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						return state.Performer.Hex.HasHexObjectOfType<Ladder>();
					}
				)
				.Build()),
		];
	}
}