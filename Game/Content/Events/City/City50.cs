using System.Collections.Generic;
using System.Linq;

public class City50 : CityEventModel<City50.ChoiceA, City50.ChoiceB>
{
	public override int Number => 50;

	public override string Text =>
		"""
		It's a particularly starry night and you decide to visit the hills for some stargazing. After making yourself comfortable in the grass, an Aesther approaches you with a glowing jar in their hand.

		"You've heard of the moon's powerful healing abilities, yes? As I travel the astral planes, I collect from the powerful glows and peddle it for institutions to study. From time to time, I'll sneak a little extra to peddle to mercenaries."

		"I have one jar left. For thirty gold, it's yours."
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Offer to buy what the Aesther is selling.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Sum(character => character.Gold) >= 30;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					"Here you go," the Aesther hands you the jar bursting with a soft, yet bright glow. "Remember, this jar contains powerful healing properties. Use them wisely!"
					""";
			}
			else
			{
				return
					"""
					You don't have enough gold and the Aesther shrugs, "Well, if you don't want to buy it, I'm sure I'll find someone else who does. Enjoy your evening!"
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainCollectiveItemEventReward(ModelDB.Item<BottledMoonlight>())
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
		public override string ChoiceText => "Ask to see the jar before committing to a purchase.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask the Aesther to allow you to closer examine the jar and the Aesther tosses it in your direction. Not anticipating the jar to be thrown your way, you fumble to catch it but it shatters to the ground and a burst of soft light emanates from the shattered glass. "You fool!" the Aesther scowls as it turns and walks away.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) => [];
	}
}