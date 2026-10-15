public class TestTube : CS2Item
{
	public override string Name => "Test Tube";
	public override int ItemNumber => 48;
	public override int ShopCount => 1;
	public override int Cost => 0;
	public override ItemType ItemType => ItemType.OneHand;
	public override ItemUseType ItemUseType => ItemUseType.Spend;
	public override bool IsSolo => true;

	protected override int AtlasIndex => 21;

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