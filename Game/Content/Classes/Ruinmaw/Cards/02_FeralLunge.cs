using System.Collections.Generic;
using Fractural.Tasks;

public class FeralLunge : RuinmawCardModel<FeralLunge.CardTop, FeralLunge.CardBottom>
{
	public override string Name => "Feral Lunge";
	public override int Level => 1;
	public override int Initiative => 31;
	protected override int AtlasIndex => 2;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithAdvantage()
				.Build()),
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
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							((MoveAbility.State)parameters.AbilityState).AdjustMoveValue(2);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
		];
	}
}