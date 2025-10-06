using System.Collections.Generic;
using Fractural.Tasks;

public class WrappedInMetal : ChainguardCardModel<WrappedInMetal.CardTop, WrappedInMetal.CardBottom>
{
	public override string Name => "Wrapped in Metal";
	public override int Level => 1;
	public override int Initiative => 82;
	protected override int AtlasIndex => 12 - 9;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(1)
				.WithRange(2)
				.WithConditions(Conditions.Stun, Chainguard.Shackle)
				.Build())
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithOnAbilityStarted(async state =>
				{
					Figure figure = await AbilityCmd.SelectFigure(state, list =>
					{
						foreach(Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 1))
						{
							if(state.Authority.EnemiesWith(figure) && figure.HasCondition(Chainguard.Shackle))
							{
								list.Add(figure);
							}
						}
					}, hintText: $"Designate an adjacent enemy with {Icons.Inline(Icons.GetCondition(Chainguard.Shackle))}");
					
					state.SetCustomValue(this, "DesignatedEnemy", figure);
					state.SetCustomValue(this, "DesignatedEnemyIsChosen", figure != null);
				})
				.Build()),

			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(3)
				.WithCustomGetTargets((state, targets) =>
				{
					targets.AddRange([state.ActionState.GetAbilityState<MoveAbility.State>(0).GetCustomValue<Figure>(this, "DesignatedEnemy")]);
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<MoveAbility.State>(0).GetCustomValue<bool>(this, "DesignatedEnemyIsChosen");
				})
				.Build())
		];
	}
}