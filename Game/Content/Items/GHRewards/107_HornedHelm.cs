using Fractural.Tasks;

public class HornedHelm : GHRewardsItem
{
	public override string Name => "Horned Helm";
	public override int ItemNumber => 107;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 12;

	private object _subscriber;
	private int _lastUseMoveCount;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			state =>
				state.Performer == Owner &&
				state.Performer.TakingTurn &&
				state.Performer.TurnMovedHexes.Count - _lastUseMoveCount >= 4,
			async state =>
			{
				await Use(async user =>
				{
					_lastUseMoveCount = state.Performer.TurnMovedHexes.Count;
					state.SingleTargetAdjustAttackValue(1);

					await GDTask.CompletedTask;
				});
			}
		);

		ScenarioEvents.FigureTurnEndedEvent.Subscribe(this, _subscriber,
			parameters => parameters.Figure == Owner,
			async parameters =>
			{
				_lastUseMoveCount = 0;

				await GDTask.CompletedTask;
			}
		);
	}

	protected override void Unsubscribe()
	{
		base.Unsubscribe();

		ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(this, _subscriber);
	}
}