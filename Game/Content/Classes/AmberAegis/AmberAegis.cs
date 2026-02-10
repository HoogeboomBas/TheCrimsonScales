using Fractural.Tasks;

public partial class AmberAegis : Character
{
	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		object subscriber = new object();

		ScenarioEvents.FigureEnteredHexEvent.Subscribe(this, subscriber,
			parameters => parameters.Hex.HasHexObjectOfType<ColonyToken>(),
			async parameters =>
			{
				ColonyToken colonyToken = parameters.Hex.GetHexObjectOfType<ColonyToken>();
				await colonyToken.Destroy();
			});
		//TODO: Destroy colony tokens on overlaytile creation in that hex
		//TODO: Make it possible to choose which hex they can move through if there are colony tokens in the way
	}
}