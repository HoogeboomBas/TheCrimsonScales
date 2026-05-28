using Fractural.Tasks;

public class KillSpecificEnemyTypeGoal : ScenarioGoal
{
	private readonly MonsterModel _monsterModel;
	private readonly bool _multiple;
	private readonly int? _specificCount;

	public KillSpecificEnemyTypeGoal(MonsterModel monsterModel, bool multiple = false, int? specificCount = null, int order = 1)
		: base(order)
	{
		_monsterModel = monsterModel;
		_multiple = multiple;
		_specificCount = specificCount;
	}

	public override string GetLabelText(RichTextParameters textParameters) =>
		_specificCount.HasValue && _specificCount.Value > 1
			? $"Kill {_specificCount} {_monsterModel.Name} enemies."
			: _multiple
				? $"Kill all {_monsterModel.Name} enemies."
				: $"Kill the {_monsterModel.Name}.";

	public override async GDTask Start()
	{
		await base.Start();

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure is Monster monster && monster.MonsterModel == _monsterModel,
			async parameters =>
			{
				await AdjustProgress(1);
			}
		);

		ScenarioEvents.RoomRevealedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				await UpdateMaxProgress();
			}
		);

		await UpdateMaxProgress();
	}

	private async GDTask UpdateMaxProgress()
	{
		if(_specificCount.HasValue)
		{
			await SetMaxProgress(_specificCount.Value);
			return;
		}

		int visibleEnemyCount = GetVisibleEnemyCount();
		int invisibleEnemyCount = GetInvisibleEnemyCount();

		await SetMaxProgress(invisibleEnemyCount > 0 ? null : (visibleEnemyCount + Progress));
	}

	private int GetVisibleEnemyCount()
	{
		int count = 0;
		foreach(Figure figure in GameController.Instance.Map.Figures)
		{
			if(figure is Monster monster && monster.MonsterModel == _monsterModel)
			{
				// Monster of this type is still alive
				count++;
			}
		}

		return count;
	}

	private int GetInvisibleEnemyCount()
	{
		int count = 0;
		string monsterModelId = _monsterModel.Id.ToString();
		foreach(MonsterSpawner monsterSpawner in GameController.Instance.Map.GetChildrenOfType<MonsterSpawner>())
		{
			if(!monsterSpawner.Revealed && monsterSpawner.MonsterModelId == monsterModelId && monsterSpawner.GetMonsterType() != MonsterType.None)
			{
				// Monster of this type still needs to be spawned
				count++;
			}
		}

		return count;
	}
}