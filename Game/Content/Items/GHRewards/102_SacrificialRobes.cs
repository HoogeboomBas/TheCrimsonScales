using Fractural.Tasks;

public class SacrificialRobes : GHRewardsItem
{
	public override string Name => "Sacrificial Robes";
	public override int ItemNumber => 102;
	public override int ShopCount => 1;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 7;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state =>
				state.Performer == Owner &&
				state.AbilityRangeType == RangeType.Range,
			apply: async state =>
			{
				await Use(async user =>
				{
					await AbilityCmd.SufferDamage(user, 3, user);

					state.AbilityAdjustAttackValue(1);

					object subscriber = new object();

					// Also add 1 to all attacks in the same action
					ScenarioEvents.AbilityStartedEvent.Subscribe(this, subscriber,
						parameters =>
							parameters.AbilityState.ActionState == state.ActionState &&
							parameters.AbilityState is AttackAbility.State,
						async parameters =>
						{
							AttackAbility.State attackAbilityState = ((AttackAbility.State)parameters.AbilityState);
							attackAbilityState.AbilityAdjustAttackValue(1);

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