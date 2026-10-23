using System.Collections.Generic;
using System.Linq;

public class CommandPopupInputAbilityCard : CommandPopupInput
{
	public override string Description =>
		"Unlocks or removes a specific ability card for a specific character.";

	public override string[] ArgumentDescriptions => ["card", "[unlock/remove]", "[class name], [card name]"];

	public override void HandleInput(List<string> arguments, CommandPopup commandPopup)
	{
		if(arguments.Count == 4)
		{
			bool unlock = true;
			if(arguments[1] == "remove")
			{
				unlock = false;
			}
			else if(arguments[1] != "unlock")
			{
				commandPopup.SetOutputErrorText("the second parameter of card must either be unlock or remove.");
				return;
			}

			SavedCharacter savedCharacter =
				BetweenScenariosController.Instance.SavedCampaign.Characters.FirstOrDefault(character =>
					character.ClassModel.Id.Entry == $"{arguments[2]}_MODEL");
			if(savedCharacter == null)
			{
				commandPopup.SetOutputErrorText(arguments[2] + " is not a valid class or class is not currently in party.");
				return;
			}

			AbilityCardModel cardModel = savedCharacter.ClassModel.AbilityCards.FirstOrDefault(card => card.Id.Entry.Equals(arguments[3]));
			if(cardModel == null)
			{
				commandPopup.SetOutputErrorText(arguments[3] + $" is not a valid ability card for {savedCharacter.ClassModel.Name}.");
				return;
			}

			if(unlock)
			{
				if(savedCharacter.AvailableAbilityCards.Any(card => card.Model.Equals(cardModel)))
				{
					commandPopup.SetOutputErrorText(cardModel.Name + $" is already available for {savedCharacter.ClassModel.Name}.");
					return;
				}

				savedCharacter.AddLevelUpCard(cardModel, forceAdd: true);
				commandPopup.SetOutputText($"unlocked {cardModel.Name} for {savedCharacter.ClassModel.Name}");
			}
			else
			{
				if(savedCharacter.AvailableAbilityCards.FirstOrDefault(card => card.Model.Equals(cardModel)) == null)
				{
					commandPopup.SetOutputErrorText(cardModel.Name +
					                                $" is not currently an available ability card for {savedCharacter.ClassModel.Name}.");
					return;
				}

				if(cardModel.Level == 1)
				{
					commandPopup.SetOutputErrorText(cardModel.Name +
					                                $" is a level 1 card and cannot be removed from {savedCharacter.ClassModel.Name}.");
					return;
				}

				savedCharacter.RemoveCard(cardModel);
				commandPopup.SetOutputText($"removed {cardModel.Name} from {savedCharacter.ClassModel.Name}");
			}
		}
		else
		{
			commandPopup.SetOutputErrorText("card takes 4 parameters.");
		}
	}
}