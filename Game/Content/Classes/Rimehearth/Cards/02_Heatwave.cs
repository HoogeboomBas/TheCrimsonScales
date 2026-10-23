using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Heatwave : RimehearthCardModel<Heatwave.CardTop, Heatwave.CardBottom>
{
	public override string Name => "Heatwave";
	public override int Level => 1;
	public override int Initiative => 19;
	protected override int AtlasIndex => 2;

	public class CardTop : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.AddCondition(state, state.Performer, Conditions.Strengthen);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return state.Performer.HasWound();
				})
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1, new HealDiamondPlus(this, new Vector2(0.49801216f, 0.22750083f)))
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3, new HealDiamondPlus(this, new Vector2(0.49801216f, 0.3157895f)))
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Wound1)
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build())
		];
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Wound1)
				.WithRange(3, new RangeSquare(this, new Vector2(0.5983488f, 0.6733842f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.ConditionAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasWound(),
						async parameters =>
						{
							await AbilityCmd.SufferDamage(parameters.AbilityState, parameters.AbilityState.Target, 2);
						}
					))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
	}
}