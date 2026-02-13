public class SurviveXRoundsScenarioGoals(int rounds, bool allSurvive = false) : ScenarioGoals
{
	public override string Text => $"Survive {rounds} rounds to win this scenario.";

	public override void Start()
	{
		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => parameters.RoundNumber >= rounds,
			async parameters =>
			{
				await Win();
			}
		);
		
		if(allSurvive)
		{
			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				parameters => parameters.Figure is Character,
				async parameters =>
				{
					await Lose();
				});
		}
	}
}