using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class NourishingMeal : RuinmawCardModel<NourishingMeal.CardTop, NourishingMeal.CardBottom>
{
	public override string Name => "Nourishing Meal";
	public override int Level => 1;
	public override int Initiative => 83;
	protected override int AtlasIndex => 10;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithConditions(Conditions.EmpowerRuinmaw, Conditions.EmpowerRuinmaw)
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return state.ActionState.GetAbilityState<AttackAbility.State>(0).KilledTargets.Count > 0;
				})
				.Build())
		];

		protected override bool Sate => true;
		protected override int XP => 2;
		protected override bool Loss => true;
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Muddle)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(0)
				.WithTarget(Target.Self)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => true,
						async parameters =>
						{
							((HealAbility.State)parameters.AbilityState).AbilityAdjustHealValue(
								RangeHelper.GetFiguresInRange(parameters.Performer.Hex, 1)
								.Where(figure => figure.EnemiesWith(parameters.Performer)).Count());
							await GDTask.CompletedTask;
						}
					)
				)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return IsSated(state.Performer);
				})
				.Build()),
		];
	}
}