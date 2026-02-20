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
		protected override List<AbilityCardAbility> GetAbilities() =>
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
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5)
				.WithMoveType(MoveType.Jump)
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Figure figure in state.Hexes
						        .SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
						        .Where(f => state.Performer.AlliedWith(f) || state.Performer.EnemiesWith(f)))
					{
						await AbilityCmd.AddCondition(state, figure, Conditions.Rupture);
					}
				})
				.Build())
		];

		protected override bool Sate => true;
		public override int XP => 2;
		public override bool Loss => true;
	}
}