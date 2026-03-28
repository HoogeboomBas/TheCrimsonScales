using System.Linq;

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

	public static bool NoEnemiesRemaining(bool countObjectives = true, bool revealedOnly = false)
	{
		if(!revealedOnly && GameController.Instance.Map.Rooms.Any(room => !room.Revealed))
		{
			return false;
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