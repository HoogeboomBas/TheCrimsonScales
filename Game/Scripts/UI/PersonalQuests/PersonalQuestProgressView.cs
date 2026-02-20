using Godot;

public partial class PersonalQuestProgressView : Control
{
	[Export]
	private PersonalQuestView _personalQuestView;
	[Export]
	private ProgressBar _progressBar;

	public void Init(SavedCharacter savedCharacter)
	{
		SavedPersonalQuest savedPersonalQuest = savedCharacter.SavedPersonalQuest;
		SetVisible(savedPersonalQuest != null);
		if(savedPersonalQuest != null)
		{
			Init(savedCharacter.ClassModel, savedCharacter.SavedPersonalQuest.Model, savedCharacter.SavedPersonalQuest.PersonalQuestData);
		}
	}

	public void Init(ClassModel classModel, PersonalQuestModel personalQuestModel, PersonalQuestData data)
	{
		_personalQuestView.SetPersonalQuest(personalQuestModel);
		int progress = data.Progress;
		int maxProgress = personalQuestModel.MaxProgress;
		float normalizedProgress = (float)progress / maxProgress;
		this.DelayedCall(() => _progressBar.Update(normalizedProgress, $"{progress}/{maxProgress}"));
		_progressBar.ProgressBarFill.SetSelfModulate(classModel.PrimaryColor);
	}
}