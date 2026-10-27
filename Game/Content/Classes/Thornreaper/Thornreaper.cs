using System.Collections.Generic;
using Fractural.Tasks;

public partial class Thornreaper : Character
{
	private ThornreaperModel _ThornreaperModel;

	private static bool _ignoreEarthConsume;
	
	public static bool GetIgnoreEarthConsume() => _ignoreEarthConsume;
	public static void SetIgnoreEarthConsume(bool value) => _ignoreEarthConsume = value;

	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		object subscriber = new object();

		_ignoreEarthConsume = false;
	}

	public override async GDTask Spawn(SavedCharacter savedCharacter, int index)
	{
		await base.Spawn(savedCharacter, index);

		_ThornreaperModel = (ThornreaperModel)savedCharacter.ClassModel;
	}
}