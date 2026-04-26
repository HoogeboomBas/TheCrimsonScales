using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class SavedPartyGoals
{
	private static readonly PartyGoalModel[] AllPartyGoals =
	[
		ModelDB.PartyGoal<BattleGoalsPartyGoal>(),
		ModelDB.PartyGoal<ExperiencePartyGoal>(),
		ModelDB.PartyGoal<SpendGoldAtShopPartyGoal>(),
		ModelDB.PartyGoal<TreasureTilePartyGoal>(),
		ModelDB.PartyGoal<SideScenarioPartyGoal>(),
	];

	[JsonProperty]
	public List<SavedPartyGoal> PartyGoals { get; private set; }

	[JsonProperty]
	public int CompletedPartyGoalCount { get; private set; }

	[JsonProperty]
	public bool CompletedEnough { get; private set; }

	public event Action CompletedPartyGoalCountChangedEvent;
	public event Action CompletedEnoughChanged;

	public SavedPartyGoals()
	{
		PartyGoals = AllPartyGoals.Select(personalQuestModel => new SavedPartyGoal(personalQuestModel)).ToList();
	}

	public void UpdateCompletedPartyGoalCount(int count)
	{
		if(CompletedPartyGoalCount == count)
		{
			return;
		}

		CompletedPartyGoalCount = count;
		CompletedPartyGoalCountChangedEvent?.Invoke();

		if(!CompletedEnough && CompletedPartyGoalCount >= 4)
		{
			CompletedEnough = true;
			CompletedEnoughChanged?.Invoke();
		}
	}
}