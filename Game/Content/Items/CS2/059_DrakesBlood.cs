public class DrakesBlood : CS2Item
{
	public override string Name => "Drake's Blood";
	public override int ItemNumber => 59;
	public override int ShopCount => 2;
	public override int Cost => 15;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 34;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					foreach(Figure figure in RangeHelper.GetFiguresInRange(character.Hex, 2))
					{
						if(character.EnemiesWith(figure))
						{
							await AbilityCmd.RemoveAllPositiveConditions(figure);
						}
					}
				});
			}
		);
	}
}