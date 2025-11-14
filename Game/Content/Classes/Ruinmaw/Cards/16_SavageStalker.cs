using System.Collections.Generic;
using Fractural.Tasks;

public class SavageStalker : RuinmawCardModel<SavageStalker.CardTop, SavageStalker.CardBottom>
{
	public override string Name => "Savage Stalker";
	public override int Level => 3;
	public override int Initiative => 38;
	protected override int AtlasIndex => 16;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => !parameters.AbilityState.Target.IsDamaged(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(3);

							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Invisible)
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
				.WithDistance(4)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithPierce(1)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return IsSated(state.Performer);
				})
				.Build()),
		];
	}
}