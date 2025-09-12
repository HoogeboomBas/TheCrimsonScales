using System.Collections.Generic;
using Fractural.Tasks;

public class ForcibleEntry : FireKnightCardModel<ForcibleEntry.CardTop, ForcibleEntry.CardBottom>
{
	public override string Name => "Forcible Entry";
	public override int Level => 1;
	public override int Initiative => 15;
	protected override int AtlasIndex => 12 - 0;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5)
				.WithPierce(2)
				.WithConditions(Conditions.Wound1)
				.Build()),
		];

		protected override IEnumerable<Element> Elements => [Element.Fire];
		protected override int XP => 2;
		protected override bool Loss => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(3).Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters =>
							state.Performer.AlliedWith(parameters.Performer) &&
							RangeHelper.Distance(parameters.Performer.Hex, state.Performer.Hex) <= 1,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();

							await GDTask.CompletedTask;
						}
					);

					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Fire))
				.Build())
		];

		protected override bool Round => true;
	}
}