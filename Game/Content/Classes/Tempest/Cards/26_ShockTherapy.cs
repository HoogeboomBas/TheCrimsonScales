using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ShockTherapy : TempestCardModel<ShockTherapy.CardTop, ShockTherapy.CardBottom>
{
	public override string Name => "Shock Therapy";
	public override int Level => 8;
	public override int Initiative => 10;
	protected override int AtlasIndex => 26;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(4, new HealDiamondPlus(this, new Vector2(0.44128302f, 0.13185596f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.65112144f, 0.13029392f)))
				.WithDuringHealSubscriptions(
				[
					LoseSparksSubscription<ScenarioEvents.DuringHeal.Parameters>(1, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustHealValue(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Heal)}")),

					LoseSparksSubscription<ScenarioEvents.DuringHeal.Parameters>(3, async parameters =>
					{
						parameters.AbilityState.AdjustTargets(1);

						await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light);
					}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Targets)}")),

					LoseSparksConsumeElementSubscription<ScenarioEvents.DuringHeal.Parameters>(1, Element.Light, async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Strengthen);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Strengthen)))),

					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Air, applyFunction: async parameters =>
					{
						parameters.AbilityState.AbilityAdjustHealValue(1);

						await GDTask.CompletedTask;
					}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Heal)}")),
				])
				.Build())
		];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.29558212f, 0.68290246f)))
				.WithRange(2)
				.WithRangeType(RangeType.Melee)
				.WithDuringAttackSubscriptions(
				[
					LoseSparksConsumeElementSubscription<ScenarioEvents.DuringAttack.Parameters>(1, Element.Light, async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Immobilize);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Immobilize)))),

					LoseSparksSubscription<ScenarioEvents.DuringAttack.Parameters>(2, async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(3);
							parameters.AbilityState.AbilityAddCondition(Conditions.Stun);

							await AbilityCmd.GainXP(parameters.Performer, 1);

							parameters.AbilityState.ActionState.SetOverrideLoss();
						},
						new TextEffectInfoView.Parameters(
							$"+3{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.GetCondition(Conditions.Stun))}, {Icons.Inline(Icons.LoseCard)}")),
				])
				.Build())
		];
	}
}