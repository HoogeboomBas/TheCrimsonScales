using Godot;

public partial class ScenarioPreviewPopupMonster : Control
{
	[Export]
	private RichTextLabel _monsterNameLabel;
	
	[Export]
	private MonsterView _monsterView;

	public void Init(MonsterModel monsterModel) 
	{
		_monsterNameLabel.SetText(monsterModel.Name);
		_monsterView.Init(monsterModel);
	}

}
