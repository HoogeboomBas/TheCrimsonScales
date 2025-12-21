using Fractural.Tasks;
using Godot;

[Tool]
public partial class MonsterSpawner : Node2D
{
	private string _monsterModelId = ModelId.None.ToString();
	private MonsterType _monsterType2Characters = MonsterType.None;
	private MonsterType _monsterType3Characters = MonsterType.None;
	private MonsterType _monsterType4Characters = MonsterType.None;

	[Export, ModelId(typeof(MonsterModel))]
	public string MonsterModelId
	{
		get => _monsterModelId;
		set
		{
			_monsterModelId = value;
			MarkDirty();
		}
	}

	[Export]
	public MonsterType MonsterType2Characters
	{
		get => _monsterType2Characters;
		set
		{
			_monsterType2Characters = value;
			MarkDirty();
		}
	}

	[Export]
	public MonsterType MonsterType3Characters
	{
		get => _monsterType3Characters;
		set
		{
			_monsterType3Characters = value;
			MarkDirty();
		}
	}

	[Export]
	public MonsterType MonsterType4Characters
	{
		get => _monsterType4Characters;
		set
		{
			_monsterType4Characters = value;
			MarkDirty();
		}
	}

	public async GDTask SpawnMonster()
	{
		QueueFree();

		MonsterType monsterType;
		int characterCount = Mathf.Max(GameController.Instance.SavedCampaign.Characters.Count, 2);
		switch(characterCount)
		{
			case 2:
				monsterType = _monsterType2Characters;
				break;
			case 3:
				monsterType = _monsterType3Characters;
				break;
			case 4:
				monsterType = _monsterType4Characters;
				break;
			default:
				return;
		}

		MonsterModel monsterModel = ModelDB.GetById<MonsterModel>(new ModelId(_monsterModelId));

		await GameController.Instance.Map.CreateMonster(monsterModel, monsterType, Map.GlobalPositionToCoords(GlobalPosition), false);
	}

	private void MarkDirty()
	{
		UpdateVisuals();
		NotifyPropertyListChanged();
	}

	private void UpdateVisuals()
	{
		MonsterModel monsterModel = ModelDB.GetById<MonsterModel>(new ModelId(_monsterModelId));

		Texture2D texture = monsterModel == null ? null : ResourceLoader.Load<Texture2D>(monsterModel?.MapIconTexturePath);
		Sprite2D sprite = GetNode<Sprite2D>("Mask/Sprite");
		sprite.Texture = texture;

		if(texture != null)
		{
			float textureWidth = texture.GetWidth();
			sprite.Scale = (250f / textureWidth) * Vector2.One;
		}

		GetNode<MonsterSpawnerIndicator>("Indicators/2Characters").UpdateVisuals(MonsterType2Characters);
		GetNode<MonsterSpawnerIndicator>("Indicators/3Characters").UpdateVisuals(MonsterType3Characters);
		GetNode<MonsterSpawnerIndicator>("Indicators/4Characters").UpdateVisuals(MonsterType4Characters);
	}
}