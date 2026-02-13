using System.Linq;

public class RingOfHaste : Prosperity5Item
{
	public override string Name => "Ring of Haste";
	public override int ItemNumber => 42;
	public override int ShopCount => 2;
	public override int Cost => 40;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 14;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeTurnEnded(
			canApply: character =>
				character == Owner &&
				Owner.Cards.Any(card => card.CardState == CardState.Hand),
			apply: async character =>
			{
				await Use(async user =>
				{
					AbilityCard card = await AbilityCmd.SelectAbilityCard(user, CardState.Hand, true,
						hintText: "Select a card to play for its bottom");

					if(card == null)
					{
						return;
					}

					await card.Bottom.Perform(user);
				});
			}
		);
	}
}