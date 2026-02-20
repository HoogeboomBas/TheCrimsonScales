using System.Collections.Generic;
using Fractural.Tasks;

public class FeastOfFlesh : RuinmawCardModel<FeastOfFlesh.CardTop, FeastOfFlesh.CardBottom>
{
	public override string Name => "Feast of Flesh";
	public override int Level => 1;
	public override int Initiative => 73;
	protected override int AtlasIndex => 8;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5)
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(
						parameters => parameters.AbilityState.Target.Health <= 2,
						async parameters =>
						{
							await AbilityCmd.KillOrExhaust(parameters.AbilityState, parameters.AbilityState.Target);
						}
					)
				)
				.Build()),
		];

		protected override bool Sate => true;
		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithTarget(Target.Self)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							((HealAbility.State)parameters.AbilityState).AbilityAddCondition(Ruinmaw.Empower);
							((HealAbility.State)parameters.AbilityState).AbilityAddCondition(Ruinmaw.Empower);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
		];
	}
}