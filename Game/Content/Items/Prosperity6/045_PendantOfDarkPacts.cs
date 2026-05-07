public class PendantOfDarkPacts : Prosperity6Item
{
	public override string Name => "Pendant of Dark Pacts";
	public override int ItemNumber => 45;
	public override int ShopCount => 2;
	public override int Cost => 75;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 4;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					for(int i = 0; i < 2; i++)
					{
						ItemModel item = await AbilityCmd.SelectItem(user, ItemState.Consumed, ItemType.Small);
						if(item == null)
						{
							break;
						}

						await AbilityCmd.RefreshItem(item);
					}

					await AbilityCmd.AddCondition(null, user, Conditions.Curse);
				});
			}
		);
	}
}