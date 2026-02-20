using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class RapidRescue : FireKnightCardModel<RapidRescue.CardTop, RapidRescue.CardBottom>
{
	public override string Name => "Rapid Rescue";
	public override int Level => 1;
	public override int Initiative => 08;
	protected override int AtlasIndex => 12 - 4;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.61780804f, 0.14749262f)))
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
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.6215359f, 0.33479995f)))
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
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder()
						.WithDistance(3, new MoveSquare(this, new Vector2(0.6178323f, 0.7326467f)))
						.Build()
				])
				.WithRange(3)
				.Build()),

			new AbilityCardAbility(GiveFireKnightItemAbility(
				state => [ModelDB.Item<FireKnightRescueAxe>(), ModelDB.Item<FireKnightRescueShield>()],
				onItemGiven: async (state, item) =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				}
			))
		];
	}
}