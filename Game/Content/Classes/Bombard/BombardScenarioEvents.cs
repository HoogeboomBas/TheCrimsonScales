public partial class ScenarioEvents
{
	public class ProjectileTokenCreated : ScenarioEvent<ProjectileTokenCreated.Parameters>
	{
		public class Parameters(Figure tokenCreator, Hex hex)
			: ParametersBase
		{
			public Figure TokenCreator { get; } = tokenCreator;

			public Hex Hex { get; } = hex;
		}
	}

	private readonly ProjectileTokenCreated _projectileTokenCreated = new ProjectileTokenCreated();
	public static ProjectileTokenCreated ProjectileTokenCreatedEvent => GameController.Instance.ScenarioEvents._projectileTokenCreated;
}