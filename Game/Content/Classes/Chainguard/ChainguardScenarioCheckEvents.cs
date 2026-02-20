public partial class ScenarioCheckEvents
{
	public class MaxShackleCountCheck : ScenarioCheckEvent<MaxShackleCountCheck.Parameters>
	{
		public class Parameters(Figure shackler)
			: ParametersBase
		{
			public Figure Shackler { get; } = shackler;

			public int MaxShackleCount { get; private set; } = 1;

			public void AdjustMaxShackleCount(int maxShackleCount)
			{
				MaxShackleCount += maxShackleCount;
			}
		}
	}

	private readonly MaxShackleCountCheck _maxShackleCountCheck = new MaxShackleCountCheck();
	public static MaxShackleCountCheck MaxShackleCountCheckEvent => GameController.Instance.ScenarioCheckEvents._maxShackleCountCheck;
}