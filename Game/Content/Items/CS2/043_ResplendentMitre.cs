using Fractural.Tasks;

public class ResplendentMitre : CS2Item
{
	public override string Name => "Resplendent Mitre";
	public override int ItemNumber => 43;
	public override int ShopCount => 1;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 16;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringHeal(
			canApply: state => state.Performer == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilityAdjustHealValue(2);

					object subscriber = new object();

					// Also add 1 to all attacks in the same action
					ScenarioEvents.AbilityStartedEvent.Subscribe(this, subscriber,
						parameters =>
							parameters.AbilityState.ActionState == state.ActionState &&
							parameters.AbilityState is HealAbility.State,
						async parameters =>
						{
							HealAbility.State attackAbilityState = ((HealAbility.State)parameters.AbilityState);
							attackAbilityState.AbilityAdjustHealValue(2);

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