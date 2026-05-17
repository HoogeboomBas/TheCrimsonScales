using System.Collections.Generic;
using Godot;

public class EnduringDarkness : HollowpactLevelUpCardModel<EnduringDarkness.CardTop, EnduringDarkness.CardBottom>
{
	public override string Name => "Enduring Darkness";
	public override int Level => 6;
	public override int Initiative => 26;
	protected override int AtlasIndex => 9;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Ward)
				.WithTarget(Target.Self)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RemoveConditionEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && parameters.ConditionModel == Conditions.Ward,
						async parameters =>
            			{
            			    parameters.SetPrevented();
							await AbilityCmd.GainXP(parameters.Figure, 1);
            			});
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.RemoveConditionEvent.Unsubscribe(state, this);
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Dark,
					effectInfoText: $"Whenever you do not have {Icons.Inline(Icons.GetCondition(Conditions.Regenerate))} this round, gain {Icons.Inline(Icons.GetCondition(Conditions.Regenerate))}"))
				.Build()),

			new AbilityCardAbility(GainVoidEnergyAbilityBuilder(2)
				.Build()),
		];

		public override bool Round => true;
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62152207f, 0.67439985f)))
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3, new HealDiamondPlus(this, new Vector2(0.44836673f, 0.7703559f)))
				.WithRange(1)
				.WithConditions(Conditions.Regenerate)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements =>
			[CardElementInfusion.Infuse(Element.Earth), CardElementInfusion.Infuse(Element.Dark)];
	}
}