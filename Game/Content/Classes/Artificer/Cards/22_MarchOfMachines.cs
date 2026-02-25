using System.Collections.Generic;

public class MarchOfMachines : ArtificerCardModel<MarchOfMachines.CardTop, MarchOfMachines.CardBottom>
{
	public override string Name => "March of Machines";
	public override int Level => 6;
	public override int Initiative => 50;
	protected override int AtlasIndex => 22;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					AbilityCmd.SummonFixedAttackRangePlusX(4).WithPierce(3).Build()
				])
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(((Character)grantState.Performer).Summons);
				})
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder().WithDistance(3).Build()
				])
				.WithTarget(Target.SelfOrAllies)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(((Character)state.Performer).Summons);
					figures.Add(state.Performer);
				})
				.Build()),
			MoveCharacterTokenBackwardAbility()
		];
	}
}