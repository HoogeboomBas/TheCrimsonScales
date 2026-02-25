using System.Collections.Generic;
using Fractural.Tasks;

public class InstantRelocationMatrix : ArtificerCardModel<InstantRelocationMatrix.CardTop, InstantRelocationMatrix.CardBottom>
{
	public override string Name => "Instant Relocation Matrix";
	public override int Level => 8;
	public override int Initiative => 69;
	protected override int AtlasIndex => 25;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					TeleportAbility.Builder().WithDistance(3).Build()
				])
				.WithTargets(2)
				.WithRange(6)
				.Build()),
			new AbilityCardAbility(ControlAbility.Builder()
				.WithAbilities(
				[
					TeleportAbility.Builder().WithDistance(3).Build()
				])
				.WithTargets(2)
				.WithRange(6)
				.WithOnAbilityStarted(async state =>
				{
					state.AdjustTargets(-state.ActionState.GetAbilityState<GrantAbility.State>(0).UniqueTargetedFigures.Count);
					await GDTask.CompletedTask;
				})
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(TeleportAbility.Builder().WithDistance(6).Build())
		];
	}
}