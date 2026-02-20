using Fractural.Tasks;

public class ThePathOfAgony : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "The Path of Agony";
	public override ClassModel ClassToUnlock => ModelDB.Class<HollowpactModel>();
	public override int MaxProgress => 13;
	protected override int AtlasIndex => 12;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.FigureKilledEvent.Subscribe(character, this,
			parameters =>
				(character.AlliedWith(parameters.Figure) || character.EnemiesWith(parameters.Figure)) &&
				GameController.Instance.Map.CurrentTurnTaker == parameters.Figure,
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}