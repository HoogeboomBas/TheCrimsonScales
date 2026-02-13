using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class IronThrust : ChainguardLevelUpCardModel<IronThrust.CardTop, IronThrust.CardBottom>
{
	public override string Name => "Iron Thrust";
	public override int Level => 2;
	public override int Initiative => 38;
	protected override int AtlasIndex => 15 - 0;

	public class CardTop : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.49266696f, 0.13226007f)))
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
				.WithAbilities(
				[
					AttackAbility.Builder()
						.WithDamage(2)
						.WithCustomGetTargets((state, figures) =>
						{
							AttackAbility.State attackAbilityState = state.ActionState.ParentActionState.GetAbilityState<AttackAbility.State>(0);
							figures.AddRange(attackAbilityState.UniqueTargetedFigures);
						})
						.Build(),
					ConditionAbility.Builder()
						.WithConditions(Conditions.Muddle)
						.WithTarget(Target.Self)
						.WithMandatory(true)
						.Build()
				])
				.WithCustomGetTargets((state, figures) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					IEnumerable<Figure> figuresPassedThrough =
						attackAbilityState.SingleTargetState.ForcedMovementHexes.SelectMany(hex => hex.GetHexObjectsOfType<Figure>());

					figures.AddRange(figuresPassedThrough.Where(figure => figure.AlliedWith(state.Performer) && figure != attackAbilityState.Target));
				})
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build()),
		];

		public override int XP => 1;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.5129702f, 0.743746f)))
				.WithMoveType(MoveType.Jump)
				.Build()),

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