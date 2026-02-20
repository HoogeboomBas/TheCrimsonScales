using System.Linq;
using Fractural.Tasks;

public class WarPick : CS1Item
{
	public override string Name => "War Pick";
	public override int ItemNumber => 13;
	public override int ShopCount => 2;
	public override int Cost => 20;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 22;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringAttack(
			canApply: state => state.Performer == Owner && state.SingleTargetRangeType == RangeType.Melee,
			apply: async state =>
			{
				await Use(async user =>
				{
					state.SingleTargetAdjustPierce(4);

					await GDTask.CompletedTask;
				});
			}
		);
	}
}