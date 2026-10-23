using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class KousFavor : IncarnateCardModel<KousFavor.CardTop, KousFavor.CardBottom>
{
	public override string Name => "Kou's Favor";
	public override int Level => 3;
	public override int Initiative => 25;
	protected override int AtlasIndex => 16;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.61930263f, 0.2f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.ItemsUsed.Any(itemModel =>
							itemModel.ItemType is ItemType.OneHand or ItemType.TwoHands && itemModel.Owner == parameters.Performer),
						async parameters =>
						{
							//TODO: Need to expand when you get +attack for multi-target attacks
							parameters.AbilityState.AbilityAddCondition(Conditions.Disarm);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}))
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Ritualist, IncarnateSpirit.Conqueror];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					//TODO: Need to expand when you get +attack for multi-target attacks
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.Performer) &&
						              RangeHelper.Distance(state.Performer.Hex, parameters.Performer.Hex) <= 2,
						async parameters =>
						{
							ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(parameters.AbilityState, this,
								canApplyParameters => canApplyParameters.AbilityState.ItemsUsed.Any(itemModel =>
									itemModel.ItemType is ItemType.OneHand or ItemType.TwoHands && itemModel.Owner == canApplyParameters.Performer),
								async _ =>
								{
									parameters.AbilityState.SingleTargetAdjustAttackValue(3);
									parameters.AbilityState.SingleTargetAdjustPierce(3);
									ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(parameters.AbilityState, this);

									await state.AdvanceUseSlot();
								});

							ScenarioEvents.AbilityEndedEvent.Subscribe(parameters.AbilityState, this,
								canApplyParameters => parameters.AbilityState == canApplyParameters.AbilityState,
								async _ =>
								{
									ScenarioEvents.AbilityEndedEvent.Unsubscribe(parameters.AbilityState, this);
									ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(parameters.AbilityState, this);

									await GDTask.CompletedTask;
								});
							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.2910257f, 0.8060942f), GainXP),
					new UseSlot(new Vector2(0.49901202f, 0.8060942f)),
					new UseSlot(new Vector2(0.7062223f, 0.8060942f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}