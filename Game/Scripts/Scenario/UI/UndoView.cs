using System.Collections.Generic;
using Godot;

public partial class UndoView : Control
{
	[Export]
	private ChoiceButton _undoButton;

	private readonly List<object> _requesters = new List<object>();
	private readonly List<object> _blockers = new List<object>();

	public override void _Ready()
	{
		base._Ready();

		_undoButton.BetterButton.Pressed += OnUndoPressed;
	}

	public void Open(object requester)
	{
		_requesters.AddIfNew(requester);

		UpdateButtons();
	}

	public void Close(object requester)
	{
		_requesters.Remove(requester);

		UpdateButtons();
	}

	public void Block(object blocker)
	{
		_blockers.AddIfNew(blocker);

		UpdateButtons();
	}

	public void UnBlock(object blocker)
	{
		_blockers.Remove(blocker);

		UpdateButtons();
	}

	private void UpdateButtons()
	{
		_undoButton.SetActive(_requesters.Count > 0 && _blockers.Count == 0 && GameController.Instance.UndoManager.CanUndo);
	}

	private void OnUndoPressed()
	{
		if(GameController.Instance.UndoManager.CanUndo)
		{
			GameController.Instance.UndoManager.Undo();
		}
	}
}