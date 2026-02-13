using System.Collections.Generic;

public abstract class MonsterModel : AbstractModel
{
	protected static int CharacterCount => GameController.Instance.SavedCampaign.Characters.Count;

	public virtual MonsterStats[] NormalLevelStats => null;
	public virtual MonsterStats[] EliteLevelStats => null;
	public virtual MonsterStats[] BossLevelStats => null;
	public virtual MonsterStats[] NamedLevelStats => null;
	public virtual MonsterModel ParentMonsterModel => null;

	public abstract string Name { get; }

	public abstract string AssetPath { get; }
	public virtual string ScenePath => "res://Scenes/Scenario/Monsters/Monster.tscn";
	public virtual string PortraitTexturePath => $"{AssetPath}/Portrait.jpg";
	public virtual string MapIconTexturePath => $"{AssetPath}/MapIcon.tres";

	public abstract int MaxStandeeCount { get; }

	public abstract IEnumerable<MonsterAbilityCardModel> Deck { get; }
}