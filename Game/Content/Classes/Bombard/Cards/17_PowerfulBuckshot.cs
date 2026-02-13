using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class PowerfulBuckshot : BombardCardModel<PowerfulBuckshot.CardTop, PowerfulBuckshot.CardBottom>
{
	public override string Name => "Powerful Buckshot";
	public override int Level => 4;
	public override int Initiative => 84;
	protected override int AtlasIndex => 17;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.4493405f, 0.24010459f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.6593378f, 0.24010459f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.Performer.TurnPerformedActionStates.Any(performedActionState =>
							performedActionState.AbilityStates.Any(state =>
								state is AttackAbility.State attackAbilityState &&
								attackAbilityState.ActionState.ParentActionState != null &&
								attackAbilityState.ActionState.ParentActionState.AbilityStates.Any(potentialProjectileAbility =>
									potentialProjectileAbility is ProjectileAbility.State &&
									attackAbilityState.DamagedFigures.Contains(parameters.AbilityState.Target)
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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.40590954f, 0.7395128f)))
				.WithTargets(2)
				.WithRange(3, new RangeSquare(this, new Vector2(0.7083057f, 0.7395128f)))
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}