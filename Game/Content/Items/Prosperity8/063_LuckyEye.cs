using System.Linq;

public class LuckyEye : Prosperity1Item
{
	public override string Name => "Lucky Eye";
	public override int ItemNumber => 63;
	public override int ShopCount => 2;
	public override int Cost => 60;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 22;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					foreach(Figure figure in RangeHelper.GetFiguresInRange(user.Hex, 1).Where(figure => user.AlliedWith(figure, true)))
					{
						await AbilityCmd.AddCondition(null, figure, Conditions.Strengthen);
					}
				});
			}
		);
	}
}