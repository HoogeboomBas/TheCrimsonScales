using Fractural.Tasks;

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

	//private int _hexMoveCount = 0;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		// ScenarioEvents.FigureTurnStartedEvent.Subscribe(this, _subscriber,
		// 	parameters => parameters.Figure == Owner,
		// 	async parameters =>
		// 	{
		// 		_hexMoveCount = 0;
		//
		// 		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this, _subscriber,
		// 			enteredHexParameters => enteredHexParameters.AbilityState is MoveAbility.State or PullSelfAbility.State,
		// 			async enteredHexParameters =>
		// 			{
		// 				_hexMoveCount++;
		//
		// 				if(_hexMoveCount == 6)
		// 				{
		// 					await Use(async () =>
		// 					{
		// 						await AbilityCmd.GainXP(Owner, 1);
		// 					});
		// 				}
		//
		// 				await GDTask.CompletedTask;
		// 			}
		// 		);
		//
		// 		await GDTask.CompletedTask;
		// 	}
		// );

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _subscriber,
			parameters => parameters.Figure == Owner,
			async parameters =>
			{
				if(parameters.Figure.TurnMovedHexCount >= 6)
				{
					await Use(async user =>
					{
						await AbilityCmd.GainXP(Owner, 1);
					});
				}
				//ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(this, _subscriber);

				await GDTask.CompletedTask;
			}
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		//ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(this, _subscriber);
		ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(this, _subscriber);
		//ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(this, _subscriber);
	}
}