using Fractural.Tasks;

public class StuddedLeather : Prosperity4Item
{
	public override string Name => "Studded Leather";
	public override int ItemNumber => 30;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 2;

	private object _subscriber;

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

					ScenarioEvents.SufferDamageEvent.Subscribe(this, _subscriber,
						canApply: parameters =>
							parameters.FromAttack && parameters.PotentialAbilityState == state && parameters.Figure == Owner,
						apply: async parameters =>
						{
							parameters.AdjustShield(1);
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