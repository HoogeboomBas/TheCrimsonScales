using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class SavedRewards
{
	[JsonProperty]
	public List<SavedReward> Rewards { get; private set; } = new List<SavedReward>();

	public void AddReward(SavedReward reward)
	{
		Rewards.Add(reward);
	}

	public void OnScenarioEnded()
	{
		for(int i = Rewards.Count - 1; i >= 0; i--)
		{
			SavedReward reward = Rewards[i];
			if(reward.MarkedForRemoval)
			{
				Rewards.RemoveAt(i);
			}
		}
	}
}