using System.Collections.Generic;
using Fractural.Tasks;

public class Heartripper : RuinmawCardModel<Heartripper.CardTop, Heartripper.CardBottom>
{
	public override string Name => "Heartripper";
	public override int Level => 8;
	public override int Initiative => 90;
	protected override int AtlasIndex => 27;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(6)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasCondition(Conditions.Rupture),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(4);

							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(5)
				.WithConditions(Conditions.EmpowerRuinmaw)
				.WithTarget(Target.Self)
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
				.WithDistance(5)
				.WithMoveType(MoveType.Jump)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							((MoveAbility.State)parameters.AbilityState).AdjustMoveValue(3);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
		];
	}
}