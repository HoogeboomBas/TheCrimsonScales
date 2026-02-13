using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class JackOfAllTrades : FireKnightLevelUpCardModel<JackOfAllTrades.CardTop, JackOfAllTrades.CardBottom>
{
	public override string Name => "Jack of All Trades";
	public override int Level => 4;
	public override int Initiative => 27;
	protected override int AtlasIndex => 11;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GiveFireKnightItemAbility(
				state => [ModelDB.Item<FireKnightScrollOfInvigoration>()],
				target: Target.SelfOrAllies,
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					return state.Performer.Hex.HasHexObjectOfType<Ladder>();
				}
			)),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.5020886f, 0.30106834f), EnhancementCostType.MultiTarget))
				.WithConditions(Conditions.Wound1)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						parameters => true,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(-1);
							parameters.AbilityState.AdjustTargets(2);

							await AbilityCmd.GainXP(parameters.Performer, 1);
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
			new AbilityCardAbility(
				MoveAbility.Builder()
					.WithDistance(4, new MoveCircle(this, new Vector2(0.6200135f, 0.6419845f)))
					.Build()),

			new AbilityCardAbility(GiveFireKnightItemAbility(
				state => [ModelDB.Item<FireKnightPikeHook>(), ModelDB.Item<FireKnightKindledTonic>(), ModelDB.Item<FireKnightExplosiveTonic>()],
				target: Target.SelfOrAllies,
				conditionalAbilityCheck: async state =>
				{
					await GDTask.CompletedTask;

					return state.Performer.Hex.HasHexObjectOfType<Ladder>();
				}
			)),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Strengthen)
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