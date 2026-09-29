using Fractural.Tasks;

public class LuminousCarapace : CS2Item
{
	public override string Name => "Luminous Carapace";
	public override int ItemNumber => 50;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.Body;
	public override ItemUseType ItemUseType => ItemUseType.Spend;
	public override bool IsSolo => true;

	protected override int AtlasIndex => 23;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		ScenarioEvents.WouldConsumeElementEvent.Subscribe(this, _subscriber,
			parameters => parameters.Consumer == Owner,
			async parameters =>
			{
				await Use(async user =>
				{
					parameters.SetConsume(false);

					await GDTask.CompletedTask;
				});
			}
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioEvents.WouldConsumeElementEvent.Unsubscribe(this, _subscriber);
	}
}