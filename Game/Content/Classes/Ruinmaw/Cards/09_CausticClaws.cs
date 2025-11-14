using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class CausticClaws : RuinmawCardModel<CausticClaws.CardTop, CausticClaws.CardBottom>
{
	public override string Name => "Caustic Claws";
	public override int Level => 1;
	public override int Initiative => 78;
	protected override int AtlasIndex => 9;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Poison1)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AdjustTargets(1);
							await GDTask.CompletedTask;
						}
					)
				)
				.WithOnAbilityEnded(async state =>
					{
						if (state.Performed && IsSated(state.Performer))
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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5)
				.WithMoveType(MoveType.Jump)
				.WithAbilityEndedSubscription(
					ScenarioEvents.AbilityEnded.Subscription.New(
						parameters => true,
						async parameters =>
						{
							foreach(Figure figure in ((MoveAbility.State)parameters.AbilityState).Hexes
								.SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
								.Where(f => parameters.Performer.AlliedWith(f) || parameters.Performer.EnemiesWith(f)))
							{
								await AbilityCmd.AddCondition(parameters.AbilityState, figure, Conditions.Rupture);
							}
						}
					)
				)
				.Build())
		];

		protected override bool Sate => true;
		protected override int XP => 2;
		protected override bool Loss => true;
	}
}