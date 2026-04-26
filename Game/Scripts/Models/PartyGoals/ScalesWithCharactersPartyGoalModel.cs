public abstract class ScalesWithCharactersPartyGoalModel : PartyGoalModel<ScalesWithCharactersPartyGoalData>
{
	public override bool ScalesWithCharacterCount => true;

	protected override int GetProgress(ScalesWithCharactersPartyGoalData partyGoalData)
	{
		int progressCount = 0;
		foreach((string guid, int progress) in partyGoalData.CharacterProgresses)
		{
			if(progress > MaxProgress)
			{
				progressCount++;
			}
		}

		return progressCount;
	}
}