using System.Collections.Generic;

public class UnlockScenario53MerchantsGuildHallReward : MerchantsGuildHallRewardModel
{
	public override string GetDescription(RichTextParameters richTextParameters) => "Unlock Scenario 53: “Cave of Currents”";

	public override List<SavedReward> GetRewards() =>
	[
		new UnlockScenarioReward(ModelDB.Scenario<Scenario053>())
	];
}