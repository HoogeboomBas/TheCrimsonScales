public class CircletOfElements : GHDesignsItem
{
	public override string Name => "Circlet of Elements";
	public override int ItemNumber => 75;
	public override int ShopCount => 2;
	public override int Cost => 25;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Spend;

	protected override int AtlasIndex => 4;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character =>
			{
				if(character != Owner)
				{
					return false;
				}

				for(int i = 0; i < 6; i++)
				{
					if(GameController.Instance.ElementManager.GetState((Element)i) > ElementState.Inert)
					{
						return true;
					}
				}

				return false;
			},
			apply: async character =>
			{
				await Use(async user =>
				{
					if((await AbilityCmd.AskConsumeWildElement(character, true)).HasValue)
					{
						await AbilityCmd.InfuseWildElement(null, character);
					}
				});
			}
		);
	}
}