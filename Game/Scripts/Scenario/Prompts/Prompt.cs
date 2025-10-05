using System;
using System.Linq;
using Fractural.Tasks;

public abstract class Prompt<TAnswer>
	where TAnswer : PromptAnswer, new()
{
	private TAnswer _answer;
	protected Figure _authority;
	private readonly Func<string> _getHintText;

	public EffectCollection EffectCollection { get; }

	protected virtual bool CanConfirm => true;
	protected virtual bool CanSkip => true;

	protected Prompt(EffectCollection effectCollection, Func<string> getHintText)
	{
		EffectCollection = effectCollection;
		_getHintText = getHintText;
	}

	/// <summary>
	/// Decide on an answer for the Prompt.
	/// Preferably just use the other overridable functions to implement functionality.
	/// </summary>
	public virtual async GDTask<TAnswer> Decide(Figure authority)
	{
		_authority = authority;

		//Prepare();
		// _cancellationToken = cancellationToken;
		_answer = null;

		Enable();

		if(_answer == null)
		{
			FullUpdateState();
		}

		// Wait until the answer has been decided
		await GDTask.WaitUntil(
			() =>
				_answer != null ||
				GameController.Instance.ResignRequested ||
				GameController.Instance.CheatWinRequested ||
				GameController.Instance.SyncedActionManager.HasSyncedAction,
			cancellationToken: GameController.CancellationToken);

		if(GameController.Instance.SyncedActionManager.HasSyncedAction)
		{
			SyncedAction syncedAction = GameController.Instance.SyncedActionManager.PopSyncedAction();
			Complete(new TAnswer()
			{
				SyncedAction = syncedAction
			});
		}

		return _answer;
	}

	/// <summary>
	/// Shows the visuals of the Prompt.
	/// Called when the Prompt is opened, and after being temporarily disabled.
	/// </summary>
	protected virtual void Enable()
	{
		GameController.Instance.ChoiceButtonsView.Open(OnContinuePressed, OnSkipPressed);
		GameController.Instance.UndoView.Open(this);
		GameController.Instance.EffectSelectionView.Open(EffectCollection, OnEffectSelected);

		if(_getHintText != null)
		{
			GameController.Instance.HintTextView.Open(_getHintText());
		}
	}

	/// <summary>
	/// (Temporarily) hides the visuals of the Prompt.
	/// </summary>
	protected virtual void Disable()
	{
		GameController.Instance.ChoiceButtonsView.Close();
		GameController.Instance.UndoView.Close(this);
		GameController.Instance.EffectSelectionView.Close();
		GameController.Instance.HintTextView.Close();
	}

	/// <summary>
	/// Final Cleanup of the Prompt. Make sure everything that is not needed anymore is hidden or deleted.
	/// </summary>
	public void Cleanup()
	{
		Disable();
	}

	/// <summary>
	/// Updates the state of the Prompt. View should be updated according to any potential choice the player has made.
	/// </summary>
	protected void FullUpdateState()
	{
		StartUpdateState();
		UpdateState();
		EndUpdateState();
	}

	/// <summary>
	/// Called before the State update cycle.
	/// </summary>
	protected virtual void StartUpdateState()
	{
	}

	/// <summary>
	/// Main State update cycle.
	/// </summary>
	protected virtual void UpdateState()
	{
	}

	/// <summary>
	/// Called after the State update cycle.
	/// </summary>
	protected virtual void EndUpdateState()
	{
		ShowOptions();
	}

	/// <summary>
	/// Shows the "Confirm", "Skip" and "Undo" buttons.
	/// </summary>
	protected virtual void ShowOptions()
	{
		//ScenarioController.Instance.ChoiceButtonsView.SetButtons(CanConfirm, CanSkip, CanUndo);
		bool finalCanConfirm =
			CanConfirm &&
			(EffectCollection == null ||
			 EffectCollection.ApplicableEffects.All(effect => effect.EffectType != EffectType.SelectableMandatory));
		GameController.Instance.ChoiceButtonsView.SetButtons(finalCanConfirm, CanSkip);
	}

	protected virtual void OnContinuePressed()
	{
		Complete();
	}

	protected virtual void OnSkipPressed()
	{
		Skip(false);
	}

	private void OnEffectSelected(Effect effect)
	{
		Complete(new TAnswer()
		{
			SelectedEffectIndex = effect.Index
		});
	}

	protected virtual TAnswer CreateAnswer()
	{
		throw new System.NotImplementedException();
	}

	/// <summary>
	/// Answer the Prompt. Finishes up everything and returns the value to the Prompt Manager.
	/// </summary>
	/// <param name="answer">The Answer.</param>
	protected void Complete(TAnswer answer)
	{
		_answer = answer;
	}

	protected void Skip(bool immediateCompletion = true)
	{
		Complete(new TAnswer()
		{
			ImmediateCompletion = immediateCompletion,
			Skipped = true
		});
	}

	protected void Complete(bool immediateCompletion = false)
	{
		TAnswer answer = CreateAnswer();
		answer.ImmediateCompletion = immediateCompletion;
		Complete(answer);
	}
}