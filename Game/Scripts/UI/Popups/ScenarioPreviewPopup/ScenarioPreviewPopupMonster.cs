using Godot;
using System;

public partial class ScenarioPreviewPopupMonster : Control
{
	[Export]
	private RichTextLabel _monsterNameLabel;
	
	[Export]
	private MonsterView _monsterView;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	public  void Init(MonsterModel monsterModel) {
		_monsterNameLabel.SetText(monsterModel.Name);
		_monsterView.Init(monsterModel);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
