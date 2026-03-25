using Fractural.Tasks;

public class Halberd : Prosperity9Item
{
	public override string Name => "Halberd";
	public override int ItemNumber => 68;
	public override int ShopCount => 2;
	public override int Cost => 75;
	public override ItemType ItemType => ItemType.TwoHands;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 8;

	private object _subscriber;

	public override void Init(Character owner)
	{
		_subscriber = new object();

		base.Init(owner);
	}

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilityAdjustRange(1);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}