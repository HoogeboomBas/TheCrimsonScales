using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class PivotAndSmash : ChainguardLevelUpCardModel<PivotAndSmash.CardTop, PivotAndSmash.CardBottom>
{
	public override string Name => "Pivot and Smash";
	public override int Level => 8;
	public override int Initiative => 28;
	protected override int AtlasIndex => 15 - 12;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SwingAbility.Builder()
				.WithSwing(4)
				.WithRange(2)
				.Build()),

			new AbilityCardAbility(OtherTargetedAbility.Builder()
				.WithCustomGetTargets((state, figures) => 
				{
					SwingAbility.State swingState = state.ActionState.GetAbilityState<SwingAbility.State>(0);

					IEnumerable<Figure> figuresPassedThrough = swingState.SingleTargetState.ForcedMovementHexes.SelectMany(hex => hex.GetHexObjectsOfType<Figure>());

					figures.AddRange(figuresPassedThrough.Where(figure => figure.EnemiesWith(state.Performer) && figure != swingState.Target));
				})
				.WithTarget(Target.Enemies)
				.WithOnAfterTargetConfirmed(async (state, figure) =>
				{
					await AbilityCmd.SufferDamage(null, figure, 2);
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<SwingAbility.State>(0).Performed;
				})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5)
				.WithCustomGetTargets((state, figures) => 
				{
					SwingAbility.State swingState = state.ActionState.GetAbilityState<SwingAbility.State>(0);

					figures.Add(swingState.Target);
				})
				.WithTarget(Target.Enemies)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<SwingAbility.State>(0).Performed;
				})
				.Build())
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(4).WithMoveType(MoveType.Jump).Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Chainguard.Shackle)
				.WithCustomGetTargets((state, figures) =>
				{
					MoveAbility.State moveAbilityState = state.ActionState.GetAbilityState<MoveAbility.State>(0);

					IEnumerable<Figure> figuresPassedThrough = moveAbilityState.Hexes.SelectMany(hex => hex.GetHexObjectsOfType<Figure>());

					figures.AddRange(figuresPassedThrough.Where(figure => figure.EnemiesWith(state.Performer)));
				})
				.Build()),

			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(3)
				.WithCustomGetTargets((state, figures) =>
				{
					Map map = GameController.Instance.Map;

					foreach(Figure figure in map.Figures)
					{
						if(figure.EnemiesWith(state.Performer) && 
							figure.HasCondition(Chainguard.Shackle) && 
							map.HasLineOfSight(figure.Hex, state.Performer.Hex))
						{
							figures.Add(figure);
						}
					}
				})
				.Build())
		];
	}
}