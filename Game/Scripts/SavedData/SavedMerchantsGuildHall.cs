using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class SavedMerchantsGuildHall
{
	private static readonly MerchantsGuildHallRewardModel[] AllRewards =
	[
		ModelDB.MerchantsGuildHallReward<GainProsperityMerchantsGuildHallReward>(),
		ModelDB.MerchantsGuildHallReward<GainProsperityMerchantsGuildHallReward>(),
		ModelDB.MerchantsGuildHallReward<GainProsperityMerchantsGuildHallReward>(),
		ModelDB.MerchantsGuildHallReward<GainProsperityMerchantsGuildHallReward>(),
		ModelDB.MerchantsGuildHallReward<GainProsperityMerchantsGuildHallReward>(),
		ModelDB.MerchantsGuildHallReward<GainProsperityMerchantsGuildHallReward>(),
		ModelDB.MerchantsGuildHallReward<GainProsperityMerchantsGuildHallReward>(),

		ModelDB.MerchantsGuildHallReward<AddCityAndRoad59MerchantsGuildHallReward>(),
		ModelDB.MerchantsGuildHallReward<AddCityAndRoad60MerchantsGuildHallReward>(),
		ModelDB.MerchantsGuildHallReward<UnlockScenario51MerchantsGuildHallReward>(),
		ModelDB.MerchantsGuildHallReward<UnlockScenario52MerchantsGuildHallReward>(),
		ModelDB.MerchantsGuildHallReward<UnlockScenario53MerchantsGuildHallReward>(),
		ModelDB.MerchantsGuildHallReward<UnlockScenario54MerchantsGuildHallReward>(),
		ModelDB.MerchantsGuildHallReward<UnlockScenario55MerchantsGuildHallReward>(),
	];

	[JsonProperty]
	public List<SavedMerchantsGuildHallReward> Rewards { get; private set; }

	[JsonProperty]
	public int CompletedScenarioCount { get; private set; }

	[JsonProperty]
	public bool Unlocked { get; private set; }

	public event Action CompletedScenarioCountChanged;

	public SavedMerchantsGuildHall()
	{
		Rewards = AllRewards.Select(rewardModel => new SavedMerchantsGuildHallReward(rewardModel)).ToList();
	}

	public void AddCompletedScenario()
	{
		CompletedScenarioCount++;
		CompletedScenarioCountChanged?.Invoke();
	}

	public void RemoveFiveCompletedScenarios()
	{
		CompletedScenarioCount -= 5;
		CompletedScenarioCountChanged?.Invoke();
	}

	public void Unlock()
	{
		Unlocked = true;
	}
}