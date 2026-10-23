using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SavvasCoreglassKnife : IncarnateCardModel<SavvasCoreglassKnife.CardTop, SavvasCoreglassKnife.CardBottom>
{
	public override string Name => "Savvas Coreglass Knife";
	public override int Level => 7;
	public override int Initiative => 81;
	protected override int AtlasIndex => 24;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.61887854f, 0.166759f)))
				.WithDuringAttackSubscriptions(
				[
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAddCondition(Conditions.Rupture);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.GetCondition(Conditions.Rupture))}")),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Air,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(2);
							parameters.AbilityState.AbilityAdjustRange(4);
							parameters.AbilityState.AbilitySetRangeType(RangeType.Range);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+2{Icons.Inline(Icons.Attack)}, +4{Icons.Inline(Icons.Range)}")),
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Earth,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAdjustPush(3);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.Push)}3")),
				])
				.Build())
		];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(6, new MoveCircle(this, new Vector2(0.6199787f, 0.67036015f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Stun)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(1)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements =>
			[CardElementInfusion.InfuseWild(), CardElementInfusion.InfuseWild(), CardElementInfusion.InfuseWild()];

		public override int XP => 2;
		public override bool Loss => true;
	}
}