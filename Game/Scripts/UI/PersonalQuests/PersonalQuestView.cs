using Godot;

public partial class PersonalQuestView : Control
{
	[Export]
	private TextureRect _textureRect;

	public PersonalQuestModel PersonalQuestModel { get; private set; }

	public void SetPersonalQuest(PersonalQuestModel personalQuestModel)
	{
		PersonalQuestModel = personalQuestModel;

		_textureRect.SetTexture(personalQuestModel.GetTexture());
	}
}