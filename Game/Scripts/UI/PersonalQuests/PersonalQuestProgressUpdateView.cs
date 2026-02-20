using Godot;

public partial class PersonalQuestProgressUpdateView : Control
{
	[Export]
	private PackedScene _personalQuestProgressViewScene;
	[Export]
	private Control _itemParent;

	public void AddItem(ClassModel classModel, PersonalQuestModel personalQuestModel, PersonalQuestData data)
	{
		PersonalQuestProgressUpdateViewItem item = _personalQuestProgressViewScene.Instantiate<PersonalQuestProgressUpdateViewItem>();
		_itemParent.AddChild(item);
		item.Init(classModel, personalQuestModel, data);
	}
}