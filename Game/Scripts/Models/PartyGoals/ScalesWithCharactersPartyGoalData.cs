using System.Collections.Generic;
using Newtonsoft.Json;

public class ScalesWithCharactersPartyGoalData : PartyGoalData
{
	[JsonProperty]
	public Dictionary<string, int> CharacterProgresses = new Dictionary<string, int>();

	public void AdjustProgress(int value, SavedCharacter savedCharacter)
	{
		string guid = savedCharacter.Guid.ToString();
		if(!CharacterProgresses.TryGetValue(guid, out int currentValue))
		{
			currentValue = 0;
			CharacterProgresses.Add(guid, currentValue);
		}

		SetProgress(currentValue + value, savedCharacter);
	}

	public void SetProgress(int value, SavedCharacter savedCharacter)
	{
		string guid = savedCharacter.Guid.ToString();
		CharacterProgresses[guid] = value;

		FireProgressChangedEvent();
	}
}