using System.Collections.Generic;
using Fractural.Tasks;

public class CrewIntegrity : FireKnightLevelUpCardModel<CrewIntegrity.CardTop, CrewIntegrity.CardBottom>
{
	public override string Name => "Crew Integrity";
	public override int Level => 3;
	public override int Initiative => 20;
	protected override int AtlasIndex => 12;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new AttackAbility(3,
				duringAttackSubscriptions:
				[
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
				],
				afterTargetConfirmedSubscriptions:
				[
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetSetHasAdvantage();

							await GDTask.CompletedTask;
						}
					)
				],
				afterAttackPerformedSubscriptions:
				[
					ScenarioEvent<ScenarioEvents.AfterAttackPerformed.Parameters>.Subscription.New(
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
				]
			))
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new GrantAbility(figure => [MoveAbility.Builder().WithDistance(3).Build()], targets: 2, range: 3,
				target: Target.SelfOrAllies | Target.SelfCountsForTargets,
				duringGrantSubscriptions:
				[
					ScenarioEvents.DuringGrant.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);

							await GDTask.CompletedTask;
						}
					)
				]
			))
		];
	}
}