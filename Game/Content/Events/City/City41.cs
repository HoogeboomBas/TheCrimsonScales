using System.Collections.Generic;
using System.Linq;

public class City41 : CityEventModel<City41.ChoiceA, City41.ChoiceB>
{
	public override int Number => 41;

	public override string Text =>
		"""
		There's a fundraising campaign for the Great Oak in the city today, and you've decided to pay your respects and make a donation.

		"A small donation would go a long way," the fundraiser bows toward you in appreciation. "A large donation would go an even longer way. Help us continue to build out the Sanctuary and you will find prosperity in all your endeavors."

		The fundraiser rubs his fingers together before extending his hand out in your direction. "So, traveler, how much will you be donating today?"
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Offer a small donation of five gold.";

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
					The fundraiser smiles as he collects the gold from the palm of your hand. "Please, allow us to bless you!" he exclaims as he ushers over several clergymen. You can never refuse a good blessing.
					""";
			}
			else
			{
				return
					"""
					You empty your pockets but don't seem to have enough coin to pay the amount you offered. "That is quite all right, traveler. It is the thought that counts most!"
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
					new AllStartScenarioWithConditionEventReward(Conditions.Bless)
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
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Donate a larger sum of twenty gold.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Sum(character => character.Gold) >= 20;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					The fundraiser jumps for joy when he sees you produce twenty gold coins from your pouch. "Marvelous! On behalf of the Great Oak, please take this blessed mitre as a token of our appreciation," the fundraiser glees as he takes the mitre off of his head and places it on yours. "Bless you, traveler, bless you!"
					""";
			}
			else
			{
				return
					"""
					You empty your pockets but don't seem to have enough coin to pay the amount you offered. "That is quite all right, traveler. It is the thought that counts most!"
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new LoseCollectiveGoldEventReward(20),
					new GainCollectiveItemEventReward(ModelDB.Item<ResplendentMitre>())
				];
			}
			else
			{
				return [];
			}
		}
	}
}