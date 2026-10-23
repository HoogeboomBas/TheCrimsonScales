using Godot;
using System;

public partial class EnemyPreview : Node
{
	[Export]
	private Label _enemyName;

	[Export]
	private TextureRect _enemyPortrait;
	
	public string EnemyName { get; set; }
	public string ResourcePath { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void Init(MonsterModel monsterModel)
	{
		Init(monsterModel.Name, monsterModel.PortraitTexturePath);
	}

	public void Init(string name, string portraitPath)
	{
		EnemyName = name;
		_enemyName.SetText(name);
		// Texture2D enemyTexture = new Texture2D();
		// enemyTexture.set
		_enemyPortrait.Texture =  GD.Load<Texture2D>(portraitPath);
		//PortraitTexturePath
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
