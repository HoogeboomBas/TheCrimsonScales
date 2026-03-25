using System.Collections.Generic;

public class StarEarring : Prosperity9Item
{
	public override string Name => "Star Earring";
	public override int ItemNumber => 69;
	public override int ShopCount => 2;
	public override int Cost => 70;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 10;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeDuringTurn(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					foreach(ItemModel item in user.Items)
					{
						if(item.ItemState == ItemState.Spent)
						{
							await AbilityCmd.RefreshItem(item);
						}
					}

					ActionState actionState = new ActionState(user,
					[
						HealAbility.Builder()
							.WithHealValue(3)
							.WithTarget(Target.Self).Build()
					]);
					await actionState.Perform();
					List<AbilityCard> selectedAbilityCard =
						await AbilityCmd.SelectAbilityCards(user, CardState.Discarded, 0, 2,
							hintText: "Select up to two discarded card to recover");

					foreach(AbilityCard abilityCard in selectedAbilityCard)
					{
						await AbilityCmd.ReturnToHand(abilityCard);
					}
				});
			}
		);
	}
}