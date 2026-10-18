public partial class ScenarioEvents
{
	public class VoidEnergySpent : ScenarioEvent<VoidEnergySpent.Parameters>
	{
		public class Parameters(Hollowpact tokenSpender, int voidSpent)
			: ParametersBase
		{
			public Figure TokenSpender { get; } = tokenSpender;
			public int VoidSpent { get; } = voidSpent;
		}
	}

	private readonly VoidEnergySpent _voidEnergySpent = new VoidEnergySpent();
	public static VoidEnergySpent VoidEnergySpentEvent => GameController.Instance.ScenarioEvents._voidEnergySpent;
}