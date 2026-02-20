using Fractural.Tasks;

public class ConjurersHand : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Conjurer's Hand";
	public override ClassModel ClassToUnlock => ModelDB.Class<SpiritCallerModel>();
	public override int MaxProgress => 10;
	protected override int AtlasIndex => 18;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		ScenarioEvents.FigureKilledEvent.Subscribe(character, this,
			parameters =>
				parameters.PotentialKiller == character &&
				parameters.Figure is Monster monster &&
				monster.IsSummon,
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}