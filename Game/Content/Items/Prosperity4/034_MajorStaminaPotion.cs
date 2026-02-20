using System.Collections.Generic;
using System.Linq;

public class MajorStaminaPotion : Prosperity4Item
{
	public override string Name => "Major Stamina Potion";
	public override int ItemNumber => 34;
	public override int ShopCount => 2;
	public override int Cost => 30;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int ColumnCount => 1;
	protected override int RowCount => 1;
	protected override string TexturePath => "res://Content/Items/Prosperity4/MajorStaminaPotion.jpg";
	protected override int AtlasIndex => 0;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character =>
				character == Owner &&
				character.Cards.Count(card => card.CardState == CardState.Discarded) > 0,
			apply: async character =>
			{
				await Use(async user =>
				{
					List<AbilityCard> selectedAbilityCard =
						await AbilityCmd.SelectAbilityCards(user, CardState.Discarded, 0, 2,
							hintText: $"Select up to two discarded card to recover");

					foreach(AbilityCard abilityCard in selectedAbilityCard)
					{
						await AbilityCmd.ReturnToHand(abilityCard);
					}
				});
			}
		);
	}
}