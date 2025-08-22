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

			new AbilityCardAbility(new GrantAbility(figure => [new ShieldAbility(1)], target: Target.SelfOrAllies))
		];

		protected override bool Round => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(3)),

			new AbilityCardAbility(new GrantAbility(figure => [new MoveAbility(3)], range: 2,
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					return state.Performer.Hex.HasHexObjectOfType<Ladder>();
				}
			)),
		];
	}
}