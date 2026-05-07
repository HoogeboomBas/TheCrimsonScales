using System.Collections.Generic;

public class AddCityAndRoad60MerchantsGuildHallReward : MerchantsGuildHallRewardModel
{
	public override string GetDescription(RichTextParameters richTextParameters) =>
		"Add City and Road Event 60 to the top of the corresponding decks";

	public override List<SavedReward> GetRewards() =>
	[
		new AddCityToTopQueueReward(ModelDB.Event<City60>()),
		new AddRoadToTopQueueReward(ModelDB.Event<Road60>()),
	];
}