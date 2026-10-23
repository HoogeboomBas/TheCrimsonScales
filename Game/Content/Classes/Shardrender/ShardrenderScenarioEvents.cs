public partial class ScenarioEvents
{
	public class CrystallizeOffLastSlot : ScenarioEvent<CrystallizeOffLastSlot.Parameters>
	{
		public class Parameters(Figure performer)
			: ParametersBase
		{
			public Figure Performer { get; } = performer;
		}
	}

	private readonly CrystallizeOffLastSlot _crystallizeOffLastSlot = new CrystallizeOffLastSlot();
	public static CrystallizeOffLastSlot CrystallizeOffLastSlotEvent => GameController.Instance.ScenarioEvents._crystallizeOffLastSlot;
}