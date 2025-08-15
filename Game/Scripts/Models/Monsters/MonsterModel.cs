﻿using System.Collections.Generic;

public abstract class MonsterModel : AbstractModel<MonsterModel>
{
	protected static int CharacterCount => GameController.Instance.SavedCampaign.Characters.Count;

	public virtual MonsterStats[] NormalLevelStats => null;
	public virtual MonsterStats[] EliteLevelStats => null;
	public virtual MonsterStats[] BossLevelStats => null;

	public abstract string Name { get; }

	public abstract string AssetPath { get; }
	public virtual string ScenePath => "res://Scenes/Scenario/Monsters/GenericMonster.tscn";
	public virtual string PortraitTexturePath => $"{AssetPath}/Portrait.jpg";
	public virtual string MapIconTexturePath => $"{AssetPath}/Icon.jpg";

	public abstract int MaxStandeeCount { get; }

	public abstract IEnumerable<MonsterAbilityCardModel> Deck { get; }
}