using System.Linq;

public class MinorStaminaPotion : Prosperity1Item
{
	public override string Name => "Minor Stamina Potion";
	public override int ItemNumber => 13;
	public override int ShopCount => 2;
	public override int Cost => 10;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 26;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner && character.Cards.Count(card => card.CardState == CardState.Discarded) > 0,
			apply: async character =>
			{
				await Use(async user =>
				{
					AbilityCard selectedAbilityCard =
						await AbilityCmd.SelectAbilityCard(character as Character, CardState.Discarded, mandatory: true, hintText: $"Select a discarded card to recover");

					await AbilityCmd.ReturnToHand(selectedAbilityCard);
				});
			}
		);
	}
}
