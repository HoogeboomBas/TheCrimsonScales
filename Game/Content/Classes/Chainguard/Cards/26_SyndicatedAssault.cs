using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class SyndicatedAssault : ChainguardLevelUpCardModel<SyndicatedAssault.CardTop, SyndicatedAssault.CardBottom>
{
	public override string Name => "Syndicated Assault";
	public override int Level => 8;
	public override int Initiative => 68;
	protected override int AtlasIndex => 15 - 13;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SwingAbility.Builder()
				.WithSwing(6)
				.WithRange(3)
				.WithOnAbilityStarted(async state =>
				{
					ScenarioCheckEvents.CanPassEnemyCheckEvent.Subscribe(state, this,
						parameters =>
							parameters.AbilityState == state &&
							parameters.Figure == state.Target,
						parameters =>
						{
							parameters.SetCanPass();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
				{
					ScenarioCheckEvents.CanPassEnemyCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantAbilityState => 
				[
					AttackAbility.Builder()
						.WithDamage(3)
						.WithCustomGetTargets((attackAbilityState, figures) => 
						{
							figures.AddRange(grantAbilityState.ActionState.GetAbilityState<SwingAbility.State>(0).UniqueTargetedFigures);
						})
						.Build()
				])
				.WithCustomGetTargets((state, figures) =>
				{
					SwingAbility.State swingState = state.ActionState.GetAbilityState<SwingAbility.State>(0);

					IEnumerable<Figure> figuresPassedThrough = swingState.SingleTargetState.ForcedMovementHexes.SelectMany(hex => hex.GetHexObjectsOfType<Figure>());

					figures.AddRange(figuresPassedThrough.Where(figure => figure.AlliedWith(state.Performer) && figure != swingState.Target));
				})
				.WithTarget(Target.Allies | Target.TargetAll)
				.Build()),
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithCustomGetTargets((state, figures) => 
				{
					figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, includeOrigin: false)
						.Where(figure => state.Performer.EnemiesWith(figure)));
				})
				.WithTarget(Target.Enemies | Target.TargetAll)
				.Build())
		];
	}
}