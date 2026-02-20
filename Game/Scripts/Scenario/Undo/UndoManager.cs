using System.Collections.Generic;
using System.Linq;

public class UndoManager
{
	private readonly List<UndoStep> _stack = new List<UndoStep>();
	private readonly List<int> _turnStackIndices = new List<int>();
	private readonly List<int> _roundStackIndices = new List<int>();

	public bool CanUndo => !GameController.Instance.ScenarioEnded && _stack.Count > 0;
	public bool CanUndoTurn => CanUndo && _turnStackIndices.Count > 0;
	public bool CanUndoRound => CanUndo && _roundStackIndices.Count > 0;

	private SavedScenario SavedScenario => GameController.Instance.SavedScenario;

	public void AddStep(UndoStep undoStep)
	{
		_stack.Add(undoStep);
	}

	public void SetTurnStart()
	{
		_turnStackIndices.Add(_stack.Count);
	}

	public void SetRoundStart()
	{
		_roundStackIndices.Add(_stack.Count);
	}

	public void Undo()
	{
		if(!CanUndo)
		{
			return;
		}

		while(CanUndo && Pop().Silent)
		{
		}

		AppController.Instance.SceneLoader.RequestSceneChange(new GameSceneRequest(GameController.Instance.SavedCampaign, true));
	}

	public void UndoTurn()
	{
		if(!CanUndoTurn)
		{
			return;
		}

		bool undoneNonSilent = false;
		while(!undoneNonSilent && CanUndoTurn)
		{
			int turnIndex = _turnStackIndices.Last();
			_turnStackIndices.RemoveLast();

			while(CanUndo && _stack.Count > turnIndex)
			{
				if(!Pop().Silent)
				{
					undoneNonSilent = true;
				}
			}
		}

		AppController.Instance.SceneLoader.RequestSceneChange(new GameSceneRequest(GameController.Instance.SavedCampaign, true));
	}

	public void UndoRound()
	{
		if(!CanUndoRound)
		{
			return;
		}

		bool undoneNonSilent = false;
		while(!undoneNonSilent && CanUndoRound)
		{
			int roundIndex = _roundStackIndices.Last();
			_roundStackIndices.RemoveLast();

			while(CanUndo && _stack.Count > roundIndex)
			{
				if(!Pop().Silent)
				{
					undoneNonSilent = true;
				}
			}
		}

		AppController.Instance.SceneLoader.RequestSceneChange(new GameSceneRequest(GameController.Instance.SavedCampaign, true));
	}

	private UndoStep Pop()
	{
		UndoStep lastStep = _stack.Last();
		lastStep.Undo(SavedScenario);
		_stack.RemoveLast();
		return lastStep;
	}
}