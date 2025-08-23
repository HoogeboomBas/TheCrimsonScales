using System.Collections.Generic;
using Fractural.Tasks;

public class JackOfAllTrades : FireKnightLevelUpCardModel<JackOfAllTrades.CardTop, JackOfAllTrades.CardBottom>
{
	public override string Name => "Jack of All Trades";
	public override int Level => 4;
	public override int Initiative => 27;
	protected override int AtlasIndex => 11;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GiveFireKnightItemAbility([ModelDB.Item<ScrollOfInvigoration>()],
				target: Target.SelfOrAllies,
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					return state.Performer.Hex.HasHexObjectOfType<Ladder>();
				}
			)),

			new AbilityCardAbility(new AttackAbility(3, conditions: [Conditions.Wound1],
				duringAttackSubscriptions:
				[
					ScenarioEvent<ScenarioEvents.DuringAttack.Parameters>.Subscription.ConsumeElement(Element.Fire,
						parameters => true,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(-1);
							parameters.AbilityState.AdjustTargets(2);

							await AbilityCmd.GainXP(parameters.Performer, 1);
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
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(4).Build()),

			new AbilityCardAbility(GiveFireKnightItemAbility([ModelDB.Item<PikeHook>(), ModelDB.Item<KindledTonic>(), ModelDB.Item<ExplosiveTonic>()],
				target: Target.SelfOrAllies,
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					return state.Performer.Hex.HasHexObjectOfType<Ladder>();
				}
			)),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Strengthen])
				.WithRange(1)
				.WithTarget(Target.Allies)
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Fire))
				.Build())
		];
	}
}