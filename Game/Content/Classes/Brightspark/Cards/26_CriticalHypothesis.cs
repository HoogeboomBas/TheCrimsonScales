using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class CriticalHypothesis : BrightsparkCardModel<CriticalHypothesis.CardTop, CriticalHypothesis.CardBottom>
{
	public override string Name => "Critical Hypothesis";
	public override int Level => 8;
	public override int Initiative => 16;
	protected override int AtlasIndex => 26;

	public class CardTop : BrightsparkCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.617037f, 0.2174603f)),
					new AttackDiamond(this, new Vector2(0.7051852f, 0.2174603f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => Math.Abs(parameters.AbilityState.Target.Initiative.MainInitiative -
						                       parameters.Performer.Initiative.MainInitiative) <= 10,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Stun);
							await AbilityCmd.InfuseWildElement(parameters.AbilityState);
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
								0 => Conditions.Wound1,
								1 => Conditions.Disarm,
								2 => Conditions.Stun,
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
					new UseSlot(new Vector2(0.29147023f, 0.8142857f)),
					new UseSlot(new Vector2(0.49925926f, 0.8142857f)),
					new UseSlot(new Vector2(0.70666665f, 0.8142857f))
				])
				.Build())
		];

		public override bool Persistent => true;
	}
}