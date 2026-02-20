using System;
using Godot;

[Tool]
public partial class MonsterSpawnerIndicator : Node2D
{
	private static readonly Color NoneColor = Colors.Black;
	private static readonly Color NormalColor = Colors.White;
	private static readonly Color EliteColor = Color.FromHtml("#edc916");
	private static readonly Color BossColor = Color.FromHtml("#bc1515");

	public void UpdateVisuals(MonsterType monsterType)
	{
		Color color = monsterType switch
		{
			MonsterType.None => NoneColor,
			MonsterType.Normal => NormalColor,
			MonsterType.Elite => EliteColor,
			MonsterType.Boss => BossColor,
			MonsterType.Named => BossColor,
			_ => throw new ArgumentOutOfRangeException()
		};

		GetNode<Sprite2D>("Sprite").SelfModulate = color;
	}
}