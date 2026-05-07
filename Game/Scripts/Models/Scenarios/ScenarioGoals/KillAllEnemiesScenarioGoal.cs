using Fractural.Tasks;

public class KillAllEnemiesScenarioGoal : ScenarioGoal
{
	private bool _enemiesToBeSpawned;
	private readonly bool _countObjectives;
	private readonly bool _revealedOnly;

	public KillAllEnemiesScenarioGoal(bool enemiesToBeSpawned = false, bool countObjectives = true, bool revealedOnly = false, int order = 0)
		: base(order)
	{
		_enemiesToBeSpawned = enemiesToBeSpawned;
		_countObjectives = countObjectives;
		_revealedOnly = revealedOnly;
	}

	public override string GetLabelText(RichTextParameters textParameters) => _revealedOnly ? "Kill all revealed enemies." : "Kill all enemies.";

	public override async GDTask Start()
	{
		await base.Start();

		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters =>
				parameters.Figure.Alignment == Alignment.Enemies && (parameters.Figure is not Objective || _countObjectives),
			async parameters =>
			{
				await AdjustProgress(1);
				await UpdateMaxProgress();
			}
		);

		ScenarioEvents.RoomRevealedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				await UpdateMaxProgress();
			}
		);

		ScenarioEvents.FigureRegisteredEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				await UpdateMaxProgress();
			}
		);

		await UpdateMaxProgress();
	}

	public async GDTask DisableEnemiesToBeSpawned()
	{
		_enemiesToBeSpawned = false;

		await UpdateMaxProgress();
	}

	private async GDTask UpdateMaxProgress()
	{
		if(_enemiesToBeSpawned)
		{
			await SetMaxProgress(null);
			return;
		}

		int visibleEnemyCount = GetVisibleEnemyCount();

		if(_revealedOnly)
		{
			await SetMaxProgress(visibleEnemyCount + Progress);
			return;
		}

		int invisibleEnemyCount = GetInvisibleEnemyCount();

		await SetMaxProgress(invisibleEnemyCount > 0 ? null : visibleEnemyCount + Progress);
	}

	private int GetVisibleEnemyCount()
	{
		int count = 0;
		foreach(Figure figure in GameController.Instance.Map.Figures)
		{
			if(figure.Alignment == Alignment.Enemies && (figure is not Objective || _countObjectives))
			{
				count++;
			}
		}

		return count;
	}

	private int GetInvisibleEnemyCount()
	{
		int count = 0;
		foreach(MonsterSpawner monsterSpawner in GameController.Instance.Map.GetChildrenOfType<MonsterSpawner>())
		{
			if(!monsterSpawner.Revealed && monsterSpawner.GetMonsterType() != MonsterType.None)
			{
				// Monster of this type still needs to be spawned
				count++;
			}
		}

		if(_countObjectives)
		{
			foreach(Objective objective in GameController.Instance.Map.GetChildrenOfType<Objective>())
			{
				if(!objective.Revealed)
				{
					count++;
				}
			}
		}

		return count;
	}
}