using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class CriticalObservation : BrightsparkCardModel<CriticalObservation.CardTop, CriticalObservation.CardBottom>
{
	public override string Name => "Critical Observation";
	public override int Level => 1;
	public override int Initiative => 20;
	protected override int AtlasIndex => 5;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.61994064f, 0.22867142f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => Math.Abs(parameters.AbilityState.Target.Initiative.MainInitiative -
						                       parameters.Performer.Initiative.MainInitiative) <= 15,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						})
				)
				.Build()),
		];
	}

	public class CardBottom : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(state.UseSlotIndex switch
							{
								0 => Conditions.Muddle,
								1 => Conditions.Poison1,
								2 => Conditions.Wound1,
								_ => throw new ArgumentOutOfRangeException()
							});
							await state.AdvanceUseSlot();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.2925002f, 0.8124983f)),
					new UseSlot(new Vector2(0.50000006f, 0.8124983f)),
					new UseSlot(new Vector2(0.70749974f, 0.8124983f))
				])
				.Build())
		];

		public override bool Persistent => true;
	}
}