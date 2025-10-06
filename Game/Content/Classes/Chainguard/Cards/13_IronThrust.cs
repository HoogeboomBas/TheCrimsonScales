using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class IronThrust : ChainguardLevelUpCardModel<IronThrust.CardTop, IronThrust.CardBottom>
{
	public override string Name => "Iron Thrust";
	public override int Level => 2;
	public override int Initiative => 38;
	protected override int AtlasIndex => 15 - 0;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithPush(3)
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
						.WithDamage(2)
						.WithCustomGetTargets((attackAbilityState, figures) => 
						{
							figures.AddRange(grantAbilityState.ActionState.GetAbilityState<AttackAbility.State>(0).UniqueTargetedFigures);
						})
						.Build()
				])
				.WithCustomGetTargets((state, figures) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					IEnumerable<Figure> figuresPassedThrough = attackAbilityState.SingleTargetState.ForcedMovementHexes.SelectMany(hex => hex.GetHexObjectsOfType<Figure>());

					figures.AddRange(figuresPassedThrough.Where(figure => figure.AlliedWith(state.Performer) && figure != attackAbilityState.Target));
				})
				.WithTarget(Target.Allies | Target.TargetAll)
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Muddle)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];

		protected override int XP => 1;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).WithMoveType(MoveType.Jump).Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Chainguard.Shackle)
				.WithCustomGetTargets((state, figures) =>
				{
					MoveAbility.State moveAbilityState = state.ActionState.GetAbilityState<MoveAbility.State>(0);

					IEnumerable<Figure> figuresPassedThrough = moveAbilityState.Hexes.SelectMany(hex => hex.GetHexObjectsOfType<Figure>());

					figures.AddRange(figuresPassedThrough.Where(figure => figure.EnemiesWith(state.Performer)));
				})
				.Build())
		];
	}
}