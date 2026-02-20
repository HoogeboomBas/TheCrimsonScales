using System.Collections.Generic;
using System.Linq;

public class City10 : CityEventModel<City10.ChoiceA, City10.ChoiceB>
{
	public override int Number => 10;

	public override string Text =>
		"""
		It's a particularly chilly day but you decide to visit the Sinking Market. During your walk, you notice a homeless Inox on the side of a merchant's stall. He's visibly smaller and frailer than most Inox you've seen, and is wrapped in a torn blanket.

		The Inox shakes a tin can in your direction and looks up to you with pleading eyes.
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Drop a few coins in his tin.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Sum(character => character.Gold) >= 5;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You take a few coins out from your pocked and drop them in his tin. The Inox flashes a wide, toothless smile and squeals out in excitement. Based on the look on his face, you can tell that you've made his day.
					""";
			}
			else
			{
				return
					"""
					You lean in to drop a few coins in his tin, but realize you don't even have enough money to feed yourself. You awkwardly walk away, leaving the Inox behind.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new LoseCollectiveGoldEventReward(5),
					new GainReputationEventReward(1)
				];
			}
			else
			{
				return [];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Grab his tin and run. Surely he wouldn't be able to catch up to you.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You reach into your pockets and pull out a handful of coins. The Inox lets out a small squeal of joy as he extends the cup further toward you. As your hand nears the tin, you drop the coins in before snatching it out of his hands. You dash as fast as you can in the opposite direction without looking back. You didn't stick around long enough to see his reaction, but you've collected enough coin for a hearty round of drinks tonight.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldEventReward(15)
		];
	}
}