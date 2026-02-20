public class KillAllEnemiesScenarioGoals(bool enemiesToBeSpawned = false, string customText = null) : ScenarioGoals
{
	public override string Text => customText ?? "Kill all enemies to win this scenario.";
	public bool EnemiesToBeSpawned = enemiesToBeSpawned;

	public override void Start()
	{
		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => !EnemiesToBeSpawned && NoEnemiesRemaining(),
			async parameters =>
			{
				await Win();
			}
		);
	}

	public static bool NoEnemiesRemaining(bool countObjectives = true)
	{
		foreach(Room room in GameController.Instance.Map.Rooms)
		{
			if(!room.Revealed)
			{
				return false;
			}
		}

		foreach(Figure figure in GameController.Instance.Map.Figures)
		{
			if(figure.Alignment == Alignment.Enemies && (figure is not Objective || countObjectives))
			{
				return false;
			}
		}

		return true;
	}
}