using System;
using Godot;

public partial class PartyGoalsPartyGoal : Control
{
	[Export]
	private TextureRect _checkmark;
	[Export]
	private RichTextLabel _label;
	[Export]
	private ProgressBar _progressBar;

	private SavedPartyGoal _savedPartyGoal;

	public bool Completed { get; private set; }

	public event Action<PartyGoalsPartyGoal> CompletedChangedEvent;

	public void Init(SavedPartyGoal savedPartyGoal)
	{
		_savedPartyGoal = savedPartyGoal;

		_savedPartyGoal.PartyGoalData.ProgressChangedEvent += OnProgressChanged;
		BetweenScenariosController.Instance.SavedCampaign.CharactersChangedEvent += OnCharactersChanged;

		Update();
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(_savedPartyGoal != null)
		{
			_savedPartyGoal.PartyGoalData.ProgressChangedEvent -= OnProgressChanged;
		}

		if(BetweenScenariosController.Instance != null)
		{
			BetweenScenariosController.Instance.SavedCampaign.CharactersChangedEvent -= OnCharactersChanged;
		}
	}

	private void Update()
	{
		int characterCount = Mathf.Max(BetweenScenariosController.Instance.SavedCampaign.Characters.Count, 2);
		_label.SetText(_savedPartyGoal.Model.GetText(characterCount));

		int maxProgress;
		if(_savedPartyGoal.Model.ScalesWithCharacterCount)
		{
			maxProgress = characterCount;
		}
		else
		{
			maxProgress = _savedPartyGoal.Model.MaxProgress;
		}

		int progress = _savedPartyGoal.Model.GetProgress(_savedPartyGoal);

		float normalizedProgress = (float)progress / maxProgress;
		_progressBar.Update(normalizedProgress, $"{progress}/{maxProgress}");

		bool prevCompleted = Completed;
		Completed = progress >= maxProgress;

		_checkmark.SetVisible(progress >= maxProgress);

		if(Completed != prevCompleted)
		{
			CompletedChangedEvent?.Invoke(this);
		}
	}

	private void OnProgressChanged(PartyGoalData partyGoalData)
	{
		Update();
	}

	private void OnCharactersChanged()
	{
		Update();
	}
}