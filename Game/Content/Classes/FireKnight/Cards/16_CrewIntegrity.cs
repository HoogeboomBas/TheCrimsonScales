using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class CrewIntegrity : FireKnightLevelUpCardModel<CrewIntegrity.CardTop, CrewIntegrity.CardBottom>
{
	public override string Name => "Crew Integrity";
	public override int Level => 3;
	public override int Initiative => 20;
	protected override int AtlasIndex => 12;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.62086433f, 0.21908909f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustRange(2);
							parameters.AbilityState.AbilitySetRangeType(RangeType.Range);

							await GDTask.CompletedTask;
						},
						effectType: EffectType.Selectable,
						canApplyMultipleTimesDuringSubscription: false,
						effectButtonParameters: new IconEffectButton.Parameters(LadderIconPath),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Range)}")
					)
				)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();

							await GDTask.CompletedTask;
						}
					)
				)
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(
						parameters => true,
						async parameters =>
						{
							foreach(Figure figure in RangeHelper.GetFiguresInRange(parameters.AbilityState.Target.Hex, 1, false))
							{
								if(parameters.Performer.AlliedWith(figure))
								{
									await AbilityCmd.AddCondition(parameters.AbilityState, figure, Conditions.Strengthen);
								}
							}
						}
					)
				)
				.Build())
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder()
						.WithDistance(3, new MoveCircle(this, new Vector2(0.6176377f, 0.8494575f)))
						.Build()
				])
				.WithTargets(2)
				.WithRange(3)
				.WithTarget(Target.SelfOrAllies | Target.SelfCountsForTargets)
				.WithDuringGrantSubscription(
					ScenarioEvents.DuringGrant.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}
					)
				)
				.Build())
		];
	}
}