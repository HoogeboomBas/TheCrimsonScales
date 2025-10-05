using System.Collections.Generic;

public class SyncedActionManager
{
	private readonly List<SyncedAction> _syncedActions = new List<SyncedAction>();

	public bool HasSyncedAction => _syncedActions.Count > 0;

	public bool PushSyncedAction(SyncedAction syncedAction)
	{
		if(_syncedActions.Count > 0)
		{
			// For now, we only allow for a single synced action at a time
			return false;
		}

		//TODO: Sync these synced actions to server for multiplayer
		_syncedActions.Add(syncedAction);

		return true;
	}

	public SyncedAction PopSyncedAction()
	{
		if(!HasSyncedAction)
		{
			return null;
		}

		SyncedAction syncedAction = _syncedActions[0];
		_syncedActions.RemoveAt(0);

		return syncedAction;
	}
}