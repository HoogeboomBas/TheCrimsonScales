using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SearchAndRescue : FireKnightLevelUpCardModel<SearchAndRescue.CardTop, SearchAndRescue.CardBottom>
{
	public override string Name => "Search And Rescue";
	public override int Level => 7;
	public override int Initiative => 18;
	protected override int AtlasIndex => 4;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithMoveType(MoveType.Jump)
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
				.WithDamage(3)
				.WithConditions(Conditions.Muddle)
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
						parameters => true,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(RangeHelper.GetFiguresInRange(parameters.AbilityState.Target.Hex, 1).Where(figure => figure.AlliedWith(parameters.Performer)).ToList().Count);

							await GDTask.CompletedTask;
						}
					)
				)
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Fire];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							MoveAbility.State moveAbilityState = (MoveAbility.State)parameters.AbilityState;
							moveAbilityState.AddJump();
							moveAbilityState.AdjustMoveValue(1);

							await GDTask.CompletedTask;
						}
					)
				)
				.Build()),
			new AbilityCardAbility(GiveFireKnightItemAbility([
				ModelDB.Item<RescueShield>(), ModelDB.Item<EmberCladding>(), ModelDB.Item<KindledTonic>()
			])),
			new AbilityCardAbility(GiveFireKnightItemAbility(((FireKnight)AbilityCard.OriginalOwner).FireKnightItems.Select(item => item.ImmutableInstance).ToList(),
				onItemGiven: async (state, item) =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				},
				conditionalAbilityCheck: state => AbilityCmd.AskConsumeElement(state.Performer, Element.Fire)
			))
		];
	}
}