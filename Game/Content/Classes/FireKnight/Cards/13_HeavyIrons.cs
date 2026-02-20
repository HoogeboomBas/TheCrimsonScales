using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class HeavyIrons : FireKnightLevelUpCardModel<HeavyIrons.CardTop, HeavyIrons.CardBottom>
{
	public override string Name => "Heavy Irons";
	public override int Level => 2;
	public override int Initiative => 79;
	protected override int AtlasIndex => 15;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.5020886f, 0.21237265f)))
				.WithConditions(Conditions.Immobilize)
				.Build()),

			new AbilityCardAbility(GiveFireKnightItemAbility(
				state => [ModelDB.Item<FireKnightRescueAxe>(), ModelDB.Item<FireKnightEmberCladding>(), ModelDB.Item<FireKnightScrollOfCharisma>()],
				onItemGiven: async (state, item) =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				}
			))
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.61811316f, 0.63576794f)))
				.WithDuringMovementSubscription(
					ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Fire,
						applyFunction: async applyParameters =>
						{
							applyParameters.AbilityState.AdjustMoveValue(2);

							await AbilityCmd.GainXP(applyParameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Move)}")
					)
				)
				.Build()),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters =>
							parameters.Performer == state.Performer &&
							RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, false).Any(figure => state.Performer.AlliedWith(figure)),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlot(new UseSlot(null))
				.Build())
		];

		public override bool Round => true;
	}
}