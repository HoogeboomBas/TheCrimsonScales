using Fractural.Tasks;

public class PiercingBow : Prosperity1Item
{
	public override string Name => "Piercing Bow";
	public override int ItemNumber => 9;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.TwoHands;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 16;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Range,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.AbilitySetIgnoresAllShields();

					await GDTask.CompletedTask;
				});
			}
		);
	}
}