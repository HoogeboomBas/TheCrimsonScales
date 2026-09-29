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

	public class SpiritAddDamageEndOfTurn : ScenarioCheckEvent<SpiritAddDamageEndOfTurn.Parameters>
	{
		public class Parameters(Spirit spirit)
			: ParametersBase
		{
			public Spirit Spirit { get; } = spirit;

			public bool AddDamage { get; private set; } = true;

			public void SetAddDamage(bool addDamage)
			{
				AddDamage = addDamage;
			}
		}
	}

	private readonly SpiritAddDamageEndOfTurn _spiritAddDamageEndOfTurn = new SpiritAddDamageEndOfTurn();
	public static SpiritAddDamageEndOfTurn SpiritAddDamageEndOfTurnEvent => GameController.Instance.ScenarioCheckEvents._spiritAddDamageEndOfTurn;
}