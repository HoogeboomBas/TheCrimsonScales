using System.Collections.Generic;

public class UnlockScenario51MerchantsGuildHallReward : MerchantsGuildHallRewardModel
{
	public override string GetDescription(RichTextParameters richTextParameters) => "Unlock Scenario 51: “Rodent Warehouse”";

	public override List<SavedReward> GetRewards() =>
	[
		new UnlockScenarioReward(ModelDB.Scenario<Scenario051>())
	];
}