using System.Collections.Generic;
using System.Linq;

public class Road28 : RoadEventModel<Road28.ChoiceA, Road28.ChoiceB>
{
	public override int Number => 28;

	public override string Text =>
		"""
		You come across a booth set up on the side of the road, where a Vermling bearing articles of clothing resembling tribal gear sits aside a collection of totems in different shapes and sizes.

		"These spiritual totems are used by our tribes in battle," the Vermling explains. "For the small price of ten gold, one of them could be yours."

		The Vermling places a totem on the table of his booth. "Today, I can sell you the Kangaroo Totem of Confusion. Would you like to buy one?"
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Offer to buy the Kangaroo Totem.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Sum(character => character.Gold) >= 10;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You agree to buy the Kangaroo Totem. "Excellent!" the Vermling rubs his hands together and hands you the totem. The totem looks different than others you've seen, with strange markings and a deformed head.

					You carry on with your journey while inspecting it, and by the time you realize it's a fake, the Vermling has already packed their bags and scurried off.
					""";
			}
			else
			{
				return
					"""
					You agree to buy the Kangaroo Totem, but quickly find you do not have enough gold.

					You tell the Vermling you won't be buying a totem today. "Not today? Not today?!" the Vermling yells as he jumps onto the table, knocking over several totems onto the floor. He begins to jump in madness and flail his arms in the air while shouting, "Not today! Not today!"

					He grabs an armful of totems and dashes off into the distance, leaving behind his booth and a small tin filled with gold.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new LoseCollectiveGoldEventReward(10)
				];
			}
			else
			{
				return
				[
					new GainCollectiveGoldEventReward(5)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Decline the offer.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tell the Vermling you won't be buying a totem today. "Not today? Not today?!" the Vermling yells as he jumps onto the table, knocking over several totems onto the floor. He begins to jump in madness and flail his arms in the air while shouting, "Not today! Not today!"

			He grabs an armful of totems and dashes off into the distance, leaving behind his booth and a small tin filled with gold.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldEventReward(5)
		];
	}
}