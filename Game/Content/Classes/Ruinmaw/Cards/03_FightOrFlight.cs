using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Fractural.Tasks;

public class FightOrFlight : RuinmawCardModel<FightOrFlight.CardTop, FightOrFlight.CardBottom>
{
	public override string Name => "Fight or Flight";
	public override int Level => 1;
	public override int Initiative => 35;
	protected override int AtlasIndex => 3;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Rupture)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilityAdjustPierce(2);
							await GDTask.CompletedTask;
						}
					)
				)
				.WithOnAbilityEndedPerformed(async state =>
					{
						if(IsSated(state.Performer))
						{
							await AbilityCmd.GainXP(state.Performer, 1);
						}
					}
				)
				.Build()),
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2)
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build())
		];
	}
}