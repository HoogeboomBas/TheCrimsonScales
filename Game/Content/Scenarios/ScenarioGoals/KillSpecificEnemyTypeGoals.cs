using System.Collections.Generic;

public class KillSpecificEnemiesTypeGoals : ScenarioGoals
{
	private readonly IEnumerable<MonsterModel> _monsterModels;

	public override string Text { get; }

	public KillSpecificEnemiesTypeGoals(MonsterModel monsterModel, string text)
	{
		_monsterModels = [monsterModel];
		Text = text;
	}

	public KillSpecificEnemiesTypeGoals(IEnumerable<MonsterModel> monsterModels, string text)
	{
		_monsterModels = monsterModels;
		Text = text;
	}

	public override void Start()
	{
		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters =>
			{
				foreach(MonsterModel monsterModel in _monsterModels)
				{
					string monsterModelId = monsterModel.Id.ToString();
					foreach(MonsterSpawner monsterSpawner in GameController.Instance.Map.GetChildrenOfType<MonsterSpawner>())
					{
						if(monsterSpawner.MonsterModelId == monsterModelId)
						{
							// Monster of this type still needs to be spawned
							return false;
						}
					}

					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						if(figure is Monster monster && monster.MonsterModel == monsterModel)
						{
							// Monster of this type is still alive
							return false;
						}
					}
				}

				return true;
			},
			async parameters =>
			{
				await Win();
			});
	}
}