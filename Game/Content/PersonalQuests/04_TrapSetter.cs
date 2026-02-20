using Fractural.Tasks;

public class TrapSetter : TheCrimsonScalesPersonalQuest<PersonalQuestData>
{
	public override string Name => "Trap Setter";
	public override ClassModel ClassToUnlock => ModelDB.Class<ChainguardModel>();
	public override int MaxProgress => 15;
	protected override int AtlasIndex => 4;

	protected override async GDTask OnScenarioSetupPhaseCompleted(Character character, PersonalQuestData personalQuestData)
	{
		await base.OnScenarioSetupPhaseCompleted(character, personalQuestData);

		// Cause enemy to trigger a trap
		ScenarioEvents.TrapTriggeredEvent.Subscribe(character, this,
			parameters =>
				GameController.Instance.Map.CurrentTurnTaker == character &&
				character.EnemiesWith(parameters.Figure) &&
				parameters.PotentialAbilityState?.Authority == character,
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);

		// Disarm a trap
		ScenarioEvents.TrapDisarmedEvent.Subscribe(character, this,
			parameters =>
				GameController.Instance.Map.CurrentTurnTaker == character &&
				parameters.PotentialDisarmer == character,
			async parameters =>
			{
				personalQuestData.AdjustProgress(1, character);

				await GDTask.CompletedTask;
			}
		);
	}
}