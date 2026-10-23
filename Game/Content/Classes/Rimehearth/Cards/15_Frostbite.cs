using System.Collections.Generic;
using System.Linq;
using Godot;

public class Frostbite : RimehearthCardModel<Frostbite.CardTop, Frostbite.CardBottom>
{
	public override string Name => "Frostbite";
	public override int Level => 3;
	public override int Initiative => 62;
	protected override int AtlasIndex => 15;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.6169745f, 0.14102024f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasCondition(Conditions.Chill),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustPierce(2);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Chill)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Ice)];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Chill)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithCustomGetTargets((_, figures) =>
				{
					figures.AddRange(GameController.Instance.Map.Figures.Where(figure => figure.HasCondition(Conditions.Chill)));
				})
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Chill)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithCustomGetTargets((_, figures) =>
				{
					foreach(Figure figure in GameController.Instance.Map.Figures.Where(figure => figure.HasCondition(Conditions.Chill)))
					{
						figures.AddRange(RangeHelper.GetFiguresInRange(figure, 1, false));
					}
				})
				.Build()),
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}