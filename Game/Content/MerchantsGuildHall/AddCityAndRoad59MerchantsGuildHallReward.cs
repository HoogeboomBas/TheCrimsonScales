using System.Collections.Generic;

public class AddCityAndRoad59MerchantsGuildHallReward : MerchantsGuildHallRewardModel
{
	public override string GetDescription(RichTextParameters richTextParameters) =>
		"Add City and Road Event 59 to the top of the corresponding decks";

	public override List<SavedReward> GetRewards() =>
	[
		new AddCityToTopQueueReward(ModelDB.Event<City59>()),
		new AddRoadToTopQueueReward(ModelDB.Event<Road59>()),
	];
}