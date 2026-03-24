using Fractural.Tasks;

public class DoomPowder : Prosperity2Item
{
	public override string Name => "Doom Powder";
	public override int ItemNumber => 62;
	public override int ShopCount => 2;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 14;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetAddCondition(Conditions.Stun);
					state.SingleTargetAddCondition(Conditions.Poison1);
					state.SingleTargetAddCondition(Conditions.Curse);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}