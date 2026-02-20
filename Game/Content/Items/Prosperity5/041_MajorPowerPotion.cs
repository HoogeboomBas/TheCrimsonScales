using Fractural.Tasks;

public class MajorPowerPotion : Prosperity5Item
{
	public override string Name => "Major Power Potion";
	public override int ItemNumber => 41;
	public override int ShopCount => 2;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 10;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilityAdjustAttackValue(2);

					object subscriber = new object();

					// Also add 1 to all attacks in the same action
					ScenarioEvents.AbilityStartedEvent.Subscribe(this, subscriber,
						parameters =>
							parameters.AbilityState.ActionState == state.ActionState &&
							parameters.AbilityState is AttackAbility.State,
						async parameters =>
						{
							AttackAbility.State attackAbilityState = ((AttackAbility.State)parameters.AbilityState);
							attackAbilityState.AbilityAdjustAttackValue(2);

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