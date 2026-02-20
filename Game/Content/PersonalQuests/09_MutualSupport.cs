using System.Linq;
using Fractural.Tasks;

public class MutualSupport : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Mutual Support";
	public override ClassModel ClassToUnlock => ModelDB.Class<FireKnightModel>();
	public override int MaxProgress => 30;
	protected override int AtlasIndex => 9;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.FigureKilledEvent.Subscribe(character, this,
			parameters =>
				parameters.PotentialKiller == character &&
				RangeHelper.GetFiguresInRange(parameters.Figure.Hex, 1, false).Any(figure => character.AlliedWith(figure)),
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}