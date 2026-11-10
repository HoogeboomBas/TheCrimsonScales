using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedScenarioProgress
{
	[JsonProperty]
	public bool Discovered { get; private set; }

	[JsonProperty]
	public bool ShownOnMap { get; private set; }

	[JsonProperty]
	public bool Completed { get; private set; }

	[JsonProperty]
	public List<int> CollectedTreasureChestNumbers { get; private set; } = new List<int>();

	[JsonProperty]
	public Dictionary<string, object> CustomValues { get; private set; } = new Dictionary<string, object>();

	public void Discover()
	{
		if(Discovered)
		{
			return;
		}

		Discovered = true;
		if(BetweenScenariosController.Instance?.ScenarioFlowchart.Showing == true)
		{
			BetweenScenariosController.Instance.ScenarioFlowchart.AnimationSequence().Forget();
		}
	}

	public void ShowOnMap()
	{
		if(ShownOnMap)
		{
			return;
		}

		ShownOnMap = true;
	}

	public void Complete()
	{
		if(Completed)
		{
			return;
		}

		Completed = true;
	}
}