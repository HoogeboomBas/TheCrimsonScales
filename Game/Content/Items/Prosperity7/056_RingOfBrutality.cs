using System.Linq;

public class RingOfBrutality : Prosperity7Item
{
	public override string Name => "Ring of Brutality";
	public override int ItemNumber => 56;
	public override int ShopCount => 2;
	public override int Cost => 50;
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
						hintText: "Select a card to play for its top");

					if(card == null)
					{
						return;
					}

					await card.Top.Perform(user);
				});
			}
		);
	}
}