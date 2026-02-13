public class StartSyncedActionStep(CardSelectionState cardSelectionState, SyncedAction syncedAction) : UndoStep
{
	public override void Undo(SavedScenario savedScenario)
	{
		int index = cardSelectionState.SyncedActions.IndexOf(syncedAction);
		cardSelectionState.SyncedActions.RemoveRange(index, cardSelectionState.SyncedActions.Count - index);
	}
}