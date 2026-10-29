using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class HardTarget : TempestCardModel<HardTarget.CardTop, HardTarget.CardBottom>
{
	public override string Name => "Hard Target";
	public override int Level => 2;
	public override int Initiative => 30;
	protected override int AtlasIndex => 14;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.5091009f, 0.19168976f)))
				.WithDuringAttackSubscriptions(
				[
					ScenarioEvents.DuringAttack.Subscription.New(parameters =>
							RangeHelper.GetFiguresInRange(parameters.Performer, 1).Count(figure => parameters.Performer.EnemiesWith(figure)) >= 2,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							await GDTask.CompletedTask;
						}),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}"))
				])
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62163085f, 0.61828256f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await AbilityCmd.AddCondition(state, state.Performer, Conditions.Dodge);
					await AbilityCmd.GainXP(state.Performer, 1);
					state.SetPerformed();
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return RangeHelper.GetFiguresInRange(state.Performer, 1).Count(figure => state.Performer.EnemiesWith(figure)) >= 2;
				})
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(state =>
					LoseSparksConditionalAbilityCheck(state.Performer, 1, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Heal)}, self")))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(state =>
					AbilityCmd.AskConsumeElement(state.Performer, Element.Air, effectInfoText: $"{Icons.Inline(Icons.Heal)}1, self"))
				.Build())
		];
	}
}