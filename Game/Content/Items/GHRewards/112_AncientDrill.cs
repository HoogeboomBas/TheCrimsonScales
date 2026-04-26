using Fractural.Tasks;

public class AncientDrill : GHRewardsItem
{
	public override string Name => "Ancient Drill";
	public override int ItemNumber => 112;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.TwoHands;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 18;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Melee,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilityAdjustAttackValue(2);
					state.AbilityAdjustPierce(2);

					object subscriber = new object();

					// Also add stun to all attacks in the same action
					ScenarioEvents.AbilityStartedEvent.Subscribe(this, subscriber,
						parameters =>
							parameters.AbilityState.ActionState == state.ActionState &&
							parameters.AbilityState is AttackAbility.State,
						async parameters =>
						{
							AttackAbility.State attackAbilityState = ((AttackAbility.State)parameters.AbilityState);
							attackAbilityState.AbilityAdjustAttackValue(2);
							attackAbilityState.AbilityAdjustPierce(2);

							await GDTask.CompletedTask;
						}
					);

					ScenarioEvents.ActionEndedEvent.Subscribe(this, subscriber,
						parameters => parameters.ActionState == state.ActionState,
						async parameters =>
						{
							ScenarioEvents.AbilityStartedEvent.Unsubscribe(this, subscriber);
							ScenarioEvents.ActionEndedEvent.Unsubscribe(this, subscriber);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}