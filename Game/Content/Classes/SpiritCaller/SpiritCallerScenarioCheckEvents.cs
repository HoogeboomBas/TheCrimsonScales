public partial class ScenarioCheckEvents
{
	public class CountsAsSpiritCheck : ScenarioCheckEvent<CountsAsSpiritCheck.Parameters>
	{
		public class Parameters(Figure figure, bool countsAsSpirit)
			: ParametersBase
		{
			public Figure Figure { get; } = figure;

			public bool CountsAsSpirit { get; private set; } = countsAsSpirit;

			public void SetCountsAsSpirit()
			{
				CountsAsSpirit = true;
			}
		}
	}

	private readonly CountsAsSpiritCheck _countsAsSpiritCheck = new CountsAsSpiritCheck();
	public static CountsAsSpiritCheck CountsAsSpiritCheckEvent => GameController.Instance.ScenarioCheckEvents._countsAsSpiritCheck;
}