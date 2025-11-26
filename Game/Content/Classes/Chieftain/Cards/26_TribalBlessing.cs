using System.Collections.Generic;
using Fractural.Tasks;
using System.Linq;

public class TribalBlessing : ChieftainCardModel<TribalBlessing.CardTop, TribalBlessing.CardBottom>
{
	public override string Name => "Tribal Blessing";
	public override int Level => 8;
	public override int Initiative => 46;
	protected override int AtlasIndex => 26;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(5)
				.WithTarget(Target.Self)
				.WithConditions(Conditions.Bless)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Earth,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AdjustTarget(Target.Allies | Target.MustTargetCharacters | Target.SelfCountsForTargets);
							applyParameters.AbilityState.AdjustTargets(1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Targets)} within {Icons.Inline(Icons.Range)}3")
					)
				)
				.WithCustomGetTargets((state, figures) => 
				{
					figures.Add(state.Performer);
					figures.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 3, false)
							.Where(figure => figure.AlliedWith(state.Performer)));
				})
				.Build())
		];

		protected override int XP => 1;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(4).Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithRange(3)
				.Build()
			)
		];
	}
}