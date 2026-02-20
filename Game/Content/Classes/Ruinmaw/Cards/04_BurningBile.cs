using System.Collections.Generic;

public class BurningBile : RuinmawCardModel<BurningBile.CardTop, BurningBile.CardBottom>
{
	public override string Name => "Burning Bile";
	public override int Level => 1;
	public override int Initiative => 45;
	protected override int AtlasIndex => 4;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRange(2)
				.WithRangeType(RangeType.Melee)
				.WithPush(2)
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							foreach(Figure figure in RangeHelper.GetFiguresInRange(parameters.AbilityState.Target.Hex, 1))
							{
								await AbilityCmd.AddCondition(parameters.AbilityState, figure, Conditions.Wound1);
							}

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}
					)
				)
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Rupture)
				.WithRange(1)
				.Build())
		];
	}
}