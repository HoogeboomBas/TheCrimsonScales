public class ScenarioSetupUndoStep : UndoStep
{
	public override void Undo(SavedScenario savedScenario)
	{
		savedScenario.ScenarioSetupState.Completed = false;
	}
}