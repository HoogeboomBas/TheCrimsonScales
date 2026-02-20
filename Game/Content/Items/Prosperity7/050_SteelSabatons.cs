using Fractural.Tasks;

public class SteelSabatons : Prosperity7Item
{
	public override string Name => "Steel Sabatons";
	public override int ItemNumber => 50;
	public override int ShopCount => 2;
	public override int Cost => 55;
	public override ItemType ItemType => ItemType.Feet;
	public override ItemUseType ItemUseType => ItemUseType.Always;
	public override int MinusOneCount => 2;

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

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _subscriber,
			parameters => parameters.Figure == Owner,
			async parameters =>
			{
				if(parameters.Figure.TurnMovedHexes.Count <= 1)
				{
					await Use(async user =>
					{
						await AbilityCmd.AddShield(user, _subscriber, 1);
					});
				}
			}
		);

		ScenarioEvents.RoundEndedEvent.Subscribe(this, _subscriber,
			parameters => true,
			async parameters =>
			{
				AbilityCmd.RemoveShield(Owner, _subscriber);

				await GDTask.CompletedTask;
			}
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		AbilityCmd.RemoveShield(Owner, _subscriber);

		ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(this, _subscriber);
	}
}