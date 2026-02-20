using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ForcibleEntry : FireKnightCardModel<ForcibleEntry.CardTop, ForcibleEntry.CardBottom>
{
	public override string Name => "Forcible Entry";
	public override int Level => 1;
	public override int Initiative => 15;
	protected override int AtlasIndex => 12 - 0;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5, new AttackDiamond(this, new Vector2(0.4324528f, 0.24680433f)))
				.WithPierce(2)
				.WithConditions(Conditions.Wound1)
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.61780804f, 0.64011794f)))
				.Build()),

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

		public override bool Round => true;
	}
}