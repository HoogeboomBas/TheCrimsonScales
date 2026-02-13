using System.Linq;
using Fractural.Tasks;

public class CruelDominion : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Cruel Dominion";
	public override ClassModel ClassToUnlock => ModelDB.Class<HollowpactModel>();
	public override int MaxProgress => 10;
	protected override int AtlasIndex => 13;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.FigureKilledEvent.Subscribe(character, this,
			parameters =>
				parameters.PotentialKiller == character &&
				parameters.Figure.Conditions.Any(condition => condition.ConditionModel.IsNegative),
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				ScenarioEvents.FigureKilledEvent.Unsubscribe(character, this);

				ScenarioEvents.ScenarioEndedEvent.Subscribe(character, this,
					scenarioEndedParameters =>
						!scenarioEndedParameters.Win,
					async scenarioEndedParameters =>
					{
						// Revert the progress if the scenario ended in a loss
						personalQuestData.AdjustProgress(-1, character);

						await GDTask.CompletedTask;
					}
				);

				await GDTask.CompletedTask;
			}
		);
	}
}