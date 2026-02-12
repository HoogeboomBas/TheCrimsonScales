using System.Linq;
using Fractural.Tasks;

public class WovenPlateArmor : CS1Item
{
	public override string Name => "Woven Plate Armor";
	public override int ItemNumber => 23;
	public override int ShopCount => 2;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Spend;
	private object _subscriber;

	protected override int AtlasIndex => 39;
	
	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeAttackAfterTargetConfirmed(
			canApply: state => state.Target == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetSetHasDisadvantage();
					SingleTargetState singleTargetState = state.SingleTargetStates.FirstOrDefault(s => s.Target == Owner);
					ScenarioEvents.SufferDamageEvent.Subscribe(this, _subscriber,
						canApply: parameters => parameters.FromAttack && parameters.PotentialAttackAbilityState == state && parameters.Figure == Owner,
						apply: async parameters =>
						{
							parameters.AdjustShield(2);
							ScenarioEvents.SufferDamageEvent.Unsubscribe(this, _subscriber);

							await GDTask.CompletedTask;
						}
					);

					ScenarioEvents.AfterAttackPerformedEvent.Subscribe(this, _subscriber,
						canApply: parameters => true,
						apply: async parameters =>
						{
							ScenarioEvents.SufferDamageEvent.Unsubscribe(this, _subscriber);
							ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(this, _subscriber);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}