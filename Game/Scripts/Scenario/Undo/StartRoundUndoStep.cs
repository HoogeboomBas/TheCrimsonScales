public class StartRoundUndoStep(CardSelectionState cardSelectionState) : UndoStep
{
	public override void Undo(SavedScenario savedScenario)
	{
		cardSelectionState.Completed = false;
	}
}