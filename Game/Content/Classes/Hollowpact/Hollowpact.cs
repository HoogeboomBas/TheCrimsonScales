using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public partial class Hollowpact : Character
{
	public static VoidEnergy VoidEnergy = new();

	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();
	}
}