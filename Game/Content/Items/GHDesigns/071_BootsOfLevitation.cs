public class BootsOfLevitation : GHDesignsItem
{
	public override string Name => "Boots of Levitation";
	public override int ItemNumber => 71;
	public override int ShopCount => 2;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 0;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		ScenarioCheckEvents.FlyingCheckEvent.Subscribe(this, _subscriber,
			parameters => parameters.Figure == Owner,
			parameters => parameters.SetFlying(true)
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioCheckEvents.FlyingCheckEvent.Unsubscribe(this, _subscriber);
	}
}