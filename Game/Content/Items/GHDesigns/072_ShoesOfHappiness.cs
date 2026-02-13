public class ShoesOfHappiness : GHDesignsItem
{
	public override string Name => "Shoes of Happiness";
	public override int ItemNumber => 72;
	public override int ShopCount => 2;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 1;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _subscriber,
			parameters => parameters.Figure == Owner,
			async parameters =>
			{
				if(parameters.Figure.TurnMovedHexes.Count >= 6)
				{
					await Use(async user =>
					{
						await AbilityCmd.GainXP(user, 1);
					});
				}
			}
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(this, _subscriber);
	}
}