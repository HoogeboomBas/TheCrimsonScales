public partial class Chieftain : Character
{
	public static bool GetIsMounted(Figure figure)
	{
		return ScenarioCheckEvents.IsMountedCheckEvent.Fire(
			new ScenarioCheckEvents.IsMountedCheck.Parameters(figure)).Mount != null;
	}

	public static Figure GetMount(Figure figure)
	{
		return ScenarioCheckEvents.IsMountedCheckEvent.Fire(
			new ScenarioCheckEvents.IsMountedCheck.Parameters(figure)).Mount;
	}
}