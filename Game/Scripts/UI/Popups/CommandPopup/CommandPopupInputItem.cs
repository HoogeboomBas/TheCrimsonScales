using System.Collections.Generic;

public class CommandPopupInputItem : CommandPopupInput
{
	public override string Description => "Unlocks or removes from the shop one copy of a specific item.";
	public override string[] ArgumentDescriptions => ["item", "[unlock/remove]", "[item name]"];

	public override void HandleInput(List<string> arguments, CommandPopup commandPopup)
	{
		if(arguments.Count == 3)
		{
			bool unlock = true;
			if(arguments[1] == "remove")
			{
				unlock = false;
			}
			else if(arguments[1] != "unlock")
			{
				commandPopup.SetOutputErrorText("the second parameter of item must either be unlock or remove.");
				return;
			}

			ItemModel itemModel = ModelDB.GetByEntry<ItemModel>(arguments[2]);
			if(itemModel == null)
			{
				commandPopup.SetOutputErrorText($"{arguments[2]} is not a valid item name.");
				return;
			}

			SavedItem savedItem = BetweenScenariosController.Instance.SavedCampaign.GetSavedItem(itemModel);

			if(unlock)
			{
				if(itemModel.ShopCount - savedItem.UnlockedCount == 0)
				{
					commandPopup.SetOutputErrorText($"{itemModel.Name} has already been unlocked the maximum number of times.");
					return;
				}

				savedItem.AddUnlocked(1);
				savedItem.AddStock(1);
				commandPopup.SetOutputText($"one copy of {itemModel.Name} has been unlocked.");
			}
			else
			{
				if(savedItem.StockCount == 0)
				{
					commandPopup.SetOutputErrorText($"{itemModel.Name} has no copies in the shop.");
					return;
				}

				savedItem.RemoveStock(1);
				savedItem.RemovedUnlocked(1);
				commandPopup.SetOutputText($"one copy of {itemModel.Name} has been removed from the shop.");
			}
		}
		else
		{
			commandPopup.SetOutputErrorText("item takes 3 parameters.");
		}
	}
}