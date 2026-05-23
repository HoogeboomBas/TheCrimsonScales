using Fractural.Tasks;

public class SurviveXRoundsScenarioGoal : ScenarioGoal
{
	private readonly int _rounds;
	private readonly bool _allSurvive;

	public SurviveXRoundsScenarioGoal(int rounds, bool allSurvive = false, int order = 0)
		: base(order)
	{
		_rounds = rounds;
		_allSurvive = allSurvive;
	}

	public override string GetLabelText(RichTextParameters textParameters) => $"Survive {_rounds} rounds.";

	public override async GDTask Start()
	{
		await base.Start();

		// ScenarioEvents.RoundEndedEvent.Subscribe(this,
		// 	parameters => parameters.RoundNumber >= _rounds,
		// 	async parameters =>
		// 	{
		// 		await Complete();
		// 	}
		// );

		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				await AdjustProgress(1);
			}
		);

		if(_allSurvive)
		{
			ScenarioEvents.FigureKilledEvent.Subscribe(this,
				parameters => parameters.Figure is Character,
				async parameters =>
				{
					await Fail();
				}
			);
		}

		await SetMaxProgress(_rounds);
	}
}