using System.Linq;
using Fractural.Tasks;

public class StaffOfEminence : Prosperity7Item
{
	public override string Name => "Staff of Eminence";
	public override int ItemNumber => 54;
	public override int ShopCount => 2;
	public override int Cost => 60;
	public override ItemType ItemType => ItemType.TwoHands;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 8;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state =>
				state.Performer == Owner &&
				state.AbilityRangeType == RangeType.Range &&
				Elements.All.Any(element => GameController.Instance.ElementManager.GetState(element) > ElementState.Inert),
			apply: async state =>
			{
				await Use(async user =>
				{
					await AbilityCmd.AskConsumeWildElement(user, true);

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