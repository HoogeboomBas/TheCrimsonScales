using System.Collections.Generic;
using Fractural.Tasks;

public class DragThroughDirt : ChainguardCardModel<DragThroughDirt.CardTop, DragThroughDirt.CardBottom>
{
	public override string Name => "Drag Through Dirt";
	public override int Level => 1;
	public override int Initiative => 30;
	protected override int AtlasIndex => 12 - 6;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherTargetedAbility.Builder()
				.WithSwing(3)
				.WithTarget(Target.Enemies)
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(state.Target.HasCondition(Chainguard.Shackle))
					{
						await AbilityCmd.AddCondition(state, state.Target, Conditions.Muddle);
					}
				})
				.Build()
			),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.Build())
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Chainguard.Shackle)
				.WithRange(1)
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(6)
				.WithMoveType(MoveType.Jump)
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
				.WithPull(6)
				.WithCustomGetTargets((state, targets) =>
				{
					targets.AddRange([state.ActionState.GetAbilityState<MoveAbility.State>(1).GetCustomValue<Figure>(this, "DesignatedEnemy")]);
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<MoveAbility.State>(1).GetCustomValue<bool>(this, "DesignatedEnemyIsChosen");
				})
				.Build())
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}
}