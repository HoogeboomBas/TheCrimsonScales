using System.Collections.Generic;
using Fractural.Tasks;

public partial class Bruiser : Character
{
	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		object subscriber = new object();
	}
}
