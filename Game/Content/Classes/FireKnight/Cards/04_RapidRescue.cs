using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class RapidRescue : FireKnightCardModel<RapidRescue.CardTop, RapidRescue.CardBottom>
{
	public override string Name => "Rapid Rescue";
	public override int Level => 1;
	public override int Initiative => 08;
	protected override int AtlasIndex => 12 - 4;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithOnAbilityStarted(async state =>
				{
					ScenarioCheckEvents.MoveCanStopAtCheckEvent.Subscribe(state.Performer, this,
						parameters =>
							parameters.AbilityState == state &&
							!RangeHelper.GetFiguresInRange(parameters.Hex, 1, false, false).Any(figure => state.Performer.AlliedWith(figure)),
						parameters =>
						{
							parameters.SetCannotStopAt();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
					{
						ScenarioCheckEvents.MoveCanStopAtCheckEvent.Unsubscribe(state.Performer, this);

						await GDTask.CompletedTask;
					}
				)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							parameters.AbilityState.AbilityAddCondition(Conditions.Muddle);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters(
							$"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.GetCondition(Conditions.Muddle))}")
					)
				)
				.Build())
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state => [MoveAbility.Builder().WithDistance(3).Build()])
				.WithRange(3)
				.Build()),

			new AbilityCardAbility(GiveFireKnightItemAbility([ModelDB.Item<RescueAxe>(), ModelDB.Item<RescueShield>()],
				onItemGiven: async (state, item) =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				}
			))
		];
	}
}