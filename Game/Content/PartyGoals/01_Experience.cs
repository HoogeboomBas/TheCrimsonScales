public class ExperiencePartyGoal : ScalesWithCharactersPartyGoalModel
{
	public override int MaxProgress => 100;

	public override string GetText(int characterCount) => $"All party members gain 100 experience each";

	protected override void SubscribeDuringDowntime(ScalesWithCharactersPartyGoalData partyGoalData)
	{
		base.SubscribeDuringDowntime(partyGoalData);

		foreach(SavedCharacter character in BetweenScenariosController.Instance.SavedCampaign.Characters)
		{
			partyGoalData.SetProgress(character.XP, character);
		}

		BetweenScenariosEvents.XPChangedEvent.Subscribe(this,
			parameters =>
			{
				partyGoalData.SetProgress(parameters.Character.XP, parameters.Character);
			}
		);
	}

	protected override void UnsubscribeDuringDowntime(ScalesWithCharactersPartyGoalData partyGoalData)
	{
		base.UnsubscribeDuringDowntime(partyGoalData);

		BetweenScenariosEvents.XPChangedEvent.Unsubscribe(this);
	}
}