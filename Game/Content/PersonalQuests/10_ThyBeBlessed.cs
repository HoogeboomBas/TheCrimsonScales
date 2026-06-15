using System.Linq;
using Fractural.Tasks;

public class ThyBeBlessed : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Thy be Blessed";
	public override ClassModel ClassToUnlock => ModelDB.Class<HierophantModel>();
	public override int MaxProgress => 12;
	protected override int AtlasIndex => 10;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.AMDCardDrawnEvent.Subscribe(character, this,
			parameters =>
				parameters.Performer == character ||
				parameters.AMDCard.Model == ModelDB.AMDCard<BlessAMDCard>() ||
				parameters.AMDCard.Model is SanctuaryCritAMDCardModel ||
				parameters.AMDCard.Model is SanctuaryRollingAMDCardModel,
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}