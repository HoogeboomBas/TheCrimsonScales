using Fractural.Tasks;

public class BattleGoalsPartyGoal : ScalesWithCharactersPartyGoalModel
{
	public override int MaxProgress => 5;

	public override string GetText(int characterCount) => $"All party members complete 5 Battle Goals each";

	protected override async GDTask OnScenarioSetupPhaseCompleted(ScalesWithCharactersPartyGoalData partyGoalData)
	{
		await base.OnScenarioSetupPhaseCompleted(partyGoalData);

		GameController.Instance.BattleGoalCompletedEvent += (character, model) =>
		{
			partyGoalData.AdjustProgress(1, character);
		};
	}
}