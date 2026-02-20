public class HeavyGreaves : Prosperity3Item
{
	public override string Name => "Heavy Greaves";
	public override int ItemNumber => 21;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Always;
	public override int MinusOneCount => 1;

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

		ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Subscribe(this, _subscriber,
			parameters => parameters.Figure == Owner,
			parameters =>
			{
				parameters.SetImmuneToForcedMovement();
			}
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Unsubscribe(this, _subscriber);
	}
}