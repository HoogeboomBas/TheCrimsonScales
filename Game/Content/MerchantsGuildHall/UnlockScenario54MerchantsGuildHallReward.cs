using System.Collections.Generic;

public class UnlockScenario54MerchantsGuildHallReward : MerchantsGuildHallRewardModel
{
	public override string GetDescription(RichTextParameters richTextParameters) => "Unlock Scenario 54: “Lair of Horrors”";

	public override List<SavedReward> GetRewards() =>
	[
		new UnlockScenarioReward(ModelDB.Scenario<Scenario054>())
	];
}