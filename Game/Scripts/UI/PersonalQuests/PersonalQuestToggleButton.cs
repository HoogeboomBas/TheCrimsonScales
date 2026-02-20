using Godot;

public partial class PersonalQuestToggleButton : ToggleButton<PersonalQuestToggleButton>
{
	[Export]
	private PersonalQuestView _personalQuestView;

	public PersonalQuestModel PersonalQuestModel { get; private set; }

	public void Init(PersonalQuestModel personalQuestModel)
	{
		PersonalQuestModel = personalQuestModel;

		_personalQuestView.SetPersonalQuest(PersonalQuestModel);

		base.Init();
	}
}