using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class PowerfulBuckshot : BombardCardModel<PowerfulBuckshot.CardTop, PowerfulBuckshot.CardBottom>
{
	public override string Name => "Powerful Buckshot";
	public override int Level => 4;
	public override int Initiative => 84;
	protected override int AtlasIndex => 17;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithRange(3)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.Performer.TurnPerformedActionStates.Any(performedActionState =>
							performedActionState.AbilityStates.Any(state =>
								state is AttackAbility.State attackAbilityState &&
								attackAbilityState.ActionState.ParentActionState != null &&
								attackAbilityState.ActionState.ParentActionState.AbilityStates.Any(potentialProjectileAbility =>
									potentialProjectileAbility is ProjectileAbility.State &&
									//TODO: Currently does not work with Unexpected Bombshell's direct suffer damage
									attackAbilityState.UniqueTargetedFigures.Contains(parameters.AbilityState.Target)
								)
							)),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(2);

							await GDTask.CompletedTask;
						}
					)
				)
				.Build())
		];
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithTargets(2)
				.WithRange(3)
				.Build())
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}
}