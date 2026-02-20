using Fractural.Tasks;

public class BarbedAxe : JotLItem
{
	public override string Name => "Barbed Axe";
	public override int ItemNumber => 34;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 7;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Melee,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilityAddCondition(Conditions.Stun);

					object subscriber = new object();

					// Also add stun to all attacks in the same action
					ScenarioEvents.AbilityStartedEvent.Subscribe(this, subscriber,
						parameters =>
							parameters.AbilityState.ActionState == state.ActionState &&
							parameters.AbilityState is AttackAbility.State,
						async parameters =>
						{
							AttackAbility.State attackAbilityState = ((AttackAbility.State)parameters.AbilityState);
							attackAbilityState.AbilityAddCondition(Conditions.Stun);

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