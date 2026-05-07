public class SideScenarioPartyGoal : PartyGoalModel<PartyGoalData>
{
	private static readonly ScenarioModel[] ScenarioModels =
	[
		ModelDB.Scenario<Scenario051>(),
		ModelDB.Scenario<Scenario052>(),
		ModelDB.Scenario<Scenario053>(),
		ModelDB.Scenario<Scenario054>(),
		ModelDB.Scenario<Scenario055>(),
	];

	public override bool ScalesWithCharacterCount => false;
	public override int MaxProgress => 1;

	public override string GetText(int characterCount) => $"The party completes one Side Scenario (#51-55)";

	protected override void SubscribeDuringDowntime(PartyGoalData partyGoalData)
	{
		base.SubscribeDuringDowntime(partyGoalData);

		int progressCount = 0;
		foreach(ScenarioModel scenarioModel in ScenarioModels)
		{
			if(BetweenScenariosController.Instance.SavedCampaign.SavedScenarioProgresses.GetScenarioProgress(scenarioModel).Completed)
			{
				progressCount++;
			}
		}

		partyGoalData.SetProgress(progressCount);
	}
}